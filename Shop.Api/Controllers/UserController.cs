using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.DataModels.CustomModels;
using Shop.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            this._userService = userService;
            
        }
        [HttpGet]
        [Route("GetCategories")]
        public IActionResult GetCategories()
        {
            var data = _userService.GetCategories();
            return Ok(data);
        }

        [HttpGet]
        [Route("GetProductByCategoryId")]
        public IActionResult GetCategories(int categoryid)
        {
            var data = _userService.GetProductByCategoryId(categoryid);
            return Ok(data);
        }
        [HttpPost]
        [Route("RegisterUser")]
        public IActionResult RegisterUser(RegisterModel register)
        {
            var data = _userService.RegisterUser(register);
            return Ok(data);
        }

        [HttpPost]
        [Route("LoginUser")]
        public IActionResult LoginUser(LoginModel loginModel)
        {
            var data = _userService.LoginUser(loginModel);
            return Ok(data);
        }
        //[HttpPost]
        //[Route("checkout")]
        //public IActionResult checkout(List<cartModel> cartModels)
        //{
        //    var data = _userService.checkout(cartModels);
        //    return Ok(data);
        //}
        [HttpGet]
        [Route("GetOrderByCustomerId")]
        public IActionResult GetOrderByCustomerId(int customerId)
        {
            var data = _userService.GetOrderByCustomerId(customerId);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetOrderDetailForCustomer")]
        public IActionResult GetOrderDetailForCustomer(int customerId, string order_number)
        {
            var data = _userService.GetOrderByCustomerId(customerId,order_number);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetShippingStatusForOrder")]
        public IActionResult GetShippingStatusForOrder(string order_number)
        {
            var data = _userService.GetShippingStausForOrder(order_number);
            return Ok(data);
        }
        [HttpPost]
        [Route("ChangePassword")]
        public IActionResult ChangePassword(PasswordModel password)
        {
            var data = _userService.ChangePassword(password);
            return Ok(data);
        }

        [HttpGet]
        [Route("CheckoutStripe")]
        public IActionResult CheckoutStripe(string cardNumber, int exp_Month, int exp_year, string cvc, decimal value)
        {          
            var data = _userService.MakePaymentStripe(cardNumber, exp_Month, exp_year, cvc, value);
            return Ok(data);
        }
        [HttpGet]
        [Route("CheckoutPayPal")]
        public IActionResult CheckoutPayPal(double total)
        {
            var url = _userService.MakePaymentPayPal(total);
            return Ok(url);
        }
        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> checkout(List<cartModel> cartModels)
        {
            ResponseModel response = new ResponseModel();
            var record = cartModels.FirstOrDefault();
            if(record != null)
            {
                if(record.PaymentMode=="CashOnDelivery")
                {
                    response = _userService.checkout(cartModels);
                }
                if (record.PaymentMode == "PayPal")
                {
                    var data = _userService.MakePaymentPayPal(record.PayPalPayment);
                    if(data != null)
                    {
                        var ref_number = data.Result.Split("&")[1];
                        cartModels.FirstOrDefault().orderReference = ref_number.Split("=")[1];
                        response = _userService.checkout(cartModels); 
                    }
                }
            }
            ////var data = _userService.checkout(cartModels);
           return Ok(response);
        }
    }
}
