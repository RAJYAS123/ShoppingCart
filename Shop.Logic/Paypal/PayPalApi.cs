
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.Paypal
{
    public class PayPalApi
    {
        public IConfiguration configration { get; }
        public PayPalApi(IConfiguration _configrataion)
        {
            configration = _configrataion;
        }
        public async Task<string> getRedirectURLToPayPAl(double total,string currency)
        {
            try
            {
                return Task.Run(async () =>
                {
                    HttpClient http = GetPayPalHttpClient();
                    PayPalAccessToken accessToken = await GetPayPalAccessTokenAsync(http);
                    PayPalCreatedResponse createdResponse = await CreatePayPalPaymentAsync(http, accessToken, total, currency);
                    var approval_url = createdResponse.links.First(x => x.rel == "approval_url").href;
                    return approval_url;
                }).Result;
            } 
            catch(Exception ex)
            {
                throw ex;
            }
        }
        private HttpClient GetPayPalHttpClient()
        {
            const string sandbox = "https://api.sandbox.paypal.com";
            var http = new HttpClient
            {
                BaseAddress = new Uri(sandbox),
                Timeout = TimeSpan.FromSeconds(30)
            };
            return http;
        }
        private  async Task<PayPalAccessToken> GetPayPalAccessTokenAsync(HttpClient httpClient)
        {
            var clientId = configration["PayPal:ClientId"];
            var secret = configration["PayPal:secret"];

            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes($"{clientId}:{secret}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            };
            request.Content = new FormUrlEncodedContent(form);
            HttpResponseMessage httpResponse = await httpClient.SendAsync(request);
            string content = await httpResponse.Content.ReadAsStringAsync();
            PayPalAccessToken accessToken = JsonConvert.DeserializeObject<PayPalAccessToken>(content);
            return accessToken;


        }
        private static async Task<PayPalCreatedResponse> CreatePayPalPaymentAsync(HttpClient httpClient, PayPalAccessToken accessToken, double total, string currency)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"v1/payments/payment");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.access_token);
            var payment = JObject.FromObject(new
            {
                intent = "sale",
                redirect_urls = new
                {
                    return_url = "http://example.com/yourredirecturl.html",
                    cancel_url= "http://example.com/yourcancelurl.html" 
                },
                payer=new {payment_method="paypal"},
                transactions = JArray.FromObject(new[] {
                    new
                    {
                        amount= new
                        {
                            total=total,
                            currency=currency
                        }
                    }
                })
            });
            
            request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await httpClient.SendAsync(request);
            string content = await httpResponse.Content.ReadAsStringAsync();
            PayPalCreatedResponse executedResponse = JsonConvert.DeserializeObject<PayPalCreatedResponse>(content);
            return executedResponse;
        }
        private static async Task<PayPalExecutedResponse> ExecutePayPalPaymentAsync(HttpClient httpClient,PayPalAccessToken accessToken,string payerId,string paymentId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"v1/payments/payment/{paymentId}/execute");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.access_token);
            var payment = JObject.FromObject(new
            {
                payer_id=payerId
            });
            request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await httpClient.SendAsync(request);
            string content = await httpResponse.Content.ReadAsStringAsync();
            PayPalExecutedResponse executedResponse = JsonConvert.DeserializeObject<PayPalExecutedResponse>(content);
            return executedResponse;
        }
    }
}
