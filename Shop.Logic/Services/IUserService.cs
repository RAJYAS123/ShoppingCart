using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.Services
{
    public interface IUserService
    {
        List<CategoryModel> GetCategories();
        List<ProductModel> GetProductByCategoryId(int categoryid);
        ResponseModel RegisterUser(RegisterModel registerModel);
        ResponseModel LoginUser(LoginModel loginModel);
        ResponseModel checkout(List<cartModel> models);
        //string GenerateOrderId();
        List<CustomerOrder> GetOrderByCustomerId(int customerId);
        List<cartModel> GetOrderByCustomerId(int customerId, string order_number);
        ResponseModel ChangePassword(PasswordModel passwordModel);
        List<string> GetShippingStausForOrder(string order_number);
        Task<string> MakePaymentStripe(string cardNumber, int exp_Month, int exp_year, string cvc, decimal value);
        Task<string> MakePaymentPayPal(double total);
    }
}
