using Microsoft.AspNetCore.Components;
using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Shop.Web.Services
{
    public class UserPanelService :IUserPanelService
    {
        private readonly HttpClient httpClient;
        private readonly ProtectedSessionStorage sesstionStorage;

        public UserPanelService(HttpClient _httpClient , ProtectedSessionStorage _sesstionStorage)
        {
            this.httpClient = _httpClient;
            this.sesstionStorage = _sesstionStorage;
        }

        public async Task<bool> IsUserLoggedIn()
        {
            bool flag = false;
            var result = await sesstionStorage.GetAsync<string>("userKey");
            if(result.Success)
            {
                flag = true;
            }
            return flag;
        }
        public async Task<List<CategoryModel>> GetCategories()
        {
            return await httpClient.GetJsonAsync<List<CategoryModel>>("api/user/GetCategories");
        }
        public async Task<List<ProductModel>> GetProductByCategoryId(int categoryid)
        {
            return await httpClient.GetJsonAsync<List<ProductModel>>("api/user/GetProductByCategoryId/?categoryid=" + categoryid);
        }
        
        public async Task<ResponseModel> RegisterUser(RegisterModel registerModel)
        {
            return await httpClient.PostJsonAsync<ResponseModel>("api/user/RegisterUser" ,registerModel);
        }
        
        public async Task<ResponseModel> LoginUser(LoginModel loginModel)
        {
           return await httpClient.PostJsonAsync<ResponseModel>("api/user/LoginUser", loginModel);
        }

        public async Task<ResponseModel> Checkout(List<cartModel> cartModels)
        {
            return await httpClient.PostJsonAsync<ResponseModel>("api/user/Checkout", cartModels);
        }
        public async Task<List<CustomerOrder>> GetOrderByCustomerId(int customerId)
        {
            return await httpClient.GetJsonAsync<List<CustomerOrder>>("api/user/GetOrderByCustomerId/?customerId="+ customerId);
        }

        public async Task<List<cartModel>> GetOrderDetailForCustomer(int customerId ,string order_number)
        {
            return await httpClient.GetJsonAsync<List<cartModel>>("api/user/GetOrderDetailForCustomer/?customerId="+customerId+ "&order_number=" + order_number);
        }
        public async Task<List<string>> GetShippingStatusForOrder(string order_number)
        {
            return await httpClient.GetJsonAsync<List<string>>("api/user/GetShippingStatusForOrder/?order_number=" + order_number);
        }
        public async Task<ResponseModel> ChangePassword(PasswordModel password)
        {
            return await httpClient.PostJsonAsync<ResponseModel>("api/user/ChangePassword", password);
        }
        

    }
}
