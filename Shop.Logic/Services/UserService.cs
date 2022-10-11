using Microsoft.Extensions.Configuration;
using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using Shop.Logic.Paypal;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Customer = Shop.DataModels.Models.Customer;

namespace Shop.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly ShoppingCartDbBlazorContext _dbContext = null;

        public TokenCardOptions Card { get; set; }
        public IConfiguration _configration { get; }


        public UserService(ShoppingCartDbBlazorContext appDbContext, IConfiguration configuration)
        {
            this._dbContext = appDbContext;
            this._configration = configuration;
        }

        public List<CategoryModel> GetCategories()
        {
            List<CategoryModel> _categoryList = new List<CategoryModel>();
            var data = _dbContext.Categories.ToList();
            foreach (var item in data)
            {
                CategoryModel categoryModel = new CategoryModel();
                categoryModel.Id = item.Id;
                categoryModel.Name = item.Name;
                _categoryList.Add(categoryModel);
            }
            return _categoryList;
        }

        public List<ProductModel> GetProductByCategoryId(int categoryid)
        {
            List<ProductModel> _productList = new List<ProductModel>();
            //var data = _dbContext.Products.ToList();
            var data = _dbContext.Products.Where(X => X.CategoryId == categoryid).ToList();
            foreach (var item in data)
            {
                ProductModel productModel = new ProductModel();
                productModel.CategoryId = item.CategoryId;
                productModel.Id = item.Id;
                productModel.Name = item.Name;
                productModel.Price = Convert.ToInt32(item.Price);
                productModel.ImageUrl = item.ImageUrl;
                productModel.Stock = item.Stock;
                _productList.Add(productModel);
            }
            return _productList;
        }

        public ResponseModel RegisterUser(RegisterModel registerModel)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var data = _dbContext.Customers.Where(X => X.Email == registerModel.EmailId).Any();
                if (!data)
                {
                    Customer _customer = new Customer();
                    _customer.Name = registerModel.Name;
                    _customer.Mobile = registerModel.MobileNo;
                    _customer.Email = registerModel.EmailId;
                    _customer.Password = registerModel.Password;
                    _dbContext.Add(_customer);
                    _dbContext.SaveChanges();

                    LoginModel loginModel = new LoginModel()
                    {
                        EmailId = registerModel.EmailId,
                        Password = registerModel.Password
                    };
                    response = LoginUser(loginModel);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Email is Already Register";
                }
                return response;

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An Error has Occured ! Try Again !!";
                return response;
            }
        }

        public ResponseModel LoginUser(LoginModel loginModel)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var userData = _dbContext.Customers.Where(x => x.Email == loginModel.EmailId).FirstOrDefault();
                if (userData != null)
                {
                    if (userData.Password == loginModel.Password)
                    {
                        response.Status = true;
                        response.Message = Convert.ToString(userData.Id) + "|" + userData.Name + "|" + userData.Email;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Your Password is Incorrect";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "Email Not Registered. Please Check Your Email Id !!";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Your Password is Incorrect";
            }
            return response;
        }
        public ResponseModel checkout(List<cartModel> models)
        {
            string OrderId = GenerateOrderId();
            var prolist = _dbContext.Products.ToList();
            try
            {
                var details = models.FirstOrDefault();
                CustomerOrder _customer = new CustomerOrder();
                _customer.CustomerId = details.UserId;
                _customer.OrderId = OrderId;
                _customer.PaymentMode = details.PaymentMode;
                _customer.ShippingAddress = details.ShippingAddress;
                _customer.ShippingCharges = details.ShippingCharges;
                _customer.SubTotal = details.SubTotal;
                _customer.Total = details.Total;
                _customer.CreatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                _customer.UpdatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                _dbContext.CustomerOrders.Add(_customer);

                foreach (var item in models)
                {
                    OrderDetail _orderDetail = new OrderDetail();
                    _orderDetail.OrderId = OrderId;
                    _orderDetail.ProductId = item.ProdctId;
                    _orderDetail.Quantity = item.Quantity;
                    _orderDetail.Price = item.Price;
                    _orderDetail.SubTotal = item.SubTotal;
                    _orderDetail.CreatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                    _orderDetail.UpdatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                    _dbContext.OrderDetails.Add(_orderDetail);

                    var selected_product = prolist.Where(x => x.Id == item.ProdctId).FirstOrDefault();
                    selected_product.Stock = selected_product.Stock - item.Quantity;
                    _dbContext.Products.Update(selected_product);

                }
                _dbContext.SaveChanges();
                ResponseModel response = new ResponseModel();
                response.Message = OrderId;
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateOrderId()
        {
            string OrderId = string.Empty;
            Random random = null;
            for (int i = 0; i < 1000; i++)
            {
                random = new Random();
                OrderId = "P" + random.Next(1, 10000000).ToString("D8");
                if (_dbContext.CustomerOrders.Where(x => x.OrderId == OrderId).Any())
                {
                    break;
                }
            }
            return OrderId;
        }

        public List<CustomerOrder> GetOrderByCustomerId(int customerId)
        {
            var _customerdata = _dbContext.CustomerOrders.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.Id).ToList();
            return _customerdata;
        }
        public List<cartModel> GetOrderByCustomerId(int customerId, string order_number)
        {
            List<cartModel> _list = new List<cartModel>();
            var Customer_Order = _dbContext.CustomerOrders.Where(x => x.CustomerId == customerId && x.OrderId == order_number).FirstOrDefault();
            if (Customer_Order != null)
            {
                var order_detail = _dbContext.OrderDetails.Where(x => x.OrderId == order_number).ToList();
                var product_list = _dbContext.Products.ToList();

                foreach (var order in order_detail)
                {
                    var prodct = product_list.Where(x => x.Id == order.ProductId).FirstOrDefault();
                    cartModel _cartModel = new cartModel();
                    _cartModel.ProdctName = prodct.Name;
                    _cartModel.Price = Convert.ToInt32(order.Price);
                    _cartModel.ProdctImage = prodct.ImageUrl;
                    _cartModel.Quantity = Convert.ToInt32(order.Quantity);
                    _list.Add(_cartModel);
                }
                _list.FirstOrDefault().ShippingAddress = Customer_Order.ShippingAddress;
                _list.FirstOrDefault().ShippingCharges = Convert.ToInt32(Customer_Order.ShippingCharges);
                _list.FirstOrDefault().SubTotal = Convert.ToInt32(Customer_Order.SubTotal);
                _list.FirstOrDefault().Total = Convert.ToInt32(Customer_Order.Total);
                _list.FirstOrDefault().PaymentMode = Customer_Order.PaymentMode;
            }
            return _list;

        }

        public ResponseModel ChangePassword(PasswordModel passwordModel)
        {
            ResponseModel response = new ResponseModel();
            var _customer = _dbContext.Customers.Where(x => x.Id == passwordModel.UserKey).FirstOrDefault();
            if (_customer != null)
            {
                _customer.Password = passwordModel.Password;
                _dbContext.Customers.Update(_customer);
                _dbContext.SaveChanges();

                response.Message = "Password Update Successfully";
            }
            else
            {
                response.Message = "User does't exist . Try Again";
            }
            return response;

        }
        public List<string> GetShippingStausForOrder(string order_number)
        {
            List<string> shipping_status = new List<string>();
            var order = _dbContext.CustomerOrders.Where(x => x.OrderId == order_number).FirstOrDefault();
            if (order != null && order.ShippingStatus != null)
            {
                shipping_status = order.ShippingStatus.Split("|").ToList();            
            }
            return shipping_status;
        }

        public async Task<string> MakePaymentStripe(string cardNumber, int exp_Month, int exp_year, string cvc, decimal value)
        {
            string response = string.Empty;
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51LrbyhSCzdYMvBX54D8nyodXX22vRA4k7rxwRvBUWsPLCnmzyCWDuKu13SBL7Jj2TGaX4tDGajpTyyHZ4m05yKuI00reUVTJmv";
                var optionToken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = cardNumber,
                        ExpMonth = Convert.ToString(exp_Month),
                        ExpYear = Convert.ToString(exp_year),
                        Cvc = cvc
                    }
                };
                var servicetoken = new TokenService();
                Token stripToken = await servicetoken.CreateAsync(optionToken);
                var customer = new Stripe.Customer
                {
                    Name = "Sivendra",
                    Address = new Address
                    {
                        Country = "India",
                        City = "Gorakhpur",
                        PostalCode = "272175",
                        Line1 = "Gonda",
                        Line2 = "Gonda Lucknow ,Up",
                        State = "UP"
                    }
                };
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(value),
                    Currency = "INR",
                    Description = "Test",
                    Source = stripToken.Id
                };
                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);
                if (charge.Paid)
                {
                    response= "Success"+"="+charge.BalanceTransactionId;
                }
                else
                {
                    response= "Fail";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> MakePaymentPayPal(double total)
        {
            try
            {
                var paypalapi = new PayPalApi(_configration);
                string url = await paypalapi.getRedirectURLToPayPAl(total, "USD");
                return url;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
