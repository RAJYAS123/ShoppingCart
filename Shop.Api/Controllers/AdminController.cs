using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.DataModels.CustomModels;
using Shop.Logic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService,IWebHostEnvironment _env)
        {
            this._adminService = adminService;
            this.env = _env;
        }

        [HttpPost]
        [Route("AdminLogin")]
        public IActionResult AdminLogin(LoginModel loginModel)             //for admin login
        {
            var data = _adminService.AdminLogin(loginModel);
            return Ok(data);
        }

        [HttpPost]
        [Route("SaveCategory")]
        public IActionResult SaveCategory(CategoryModel categoryModel)
        {
            var data = _adminService.SaveCategory(categoryModel);
            return Ok(data);
        }
        [HttpGet]
        [Route("GetCategories")]
        public IActionResult GetCategories()       //for get category
        {
            var data = _adminService.GetCategories();
            return Ok(data);
        }
        [HttpPost]
        [Route("UpdateCategory")]
        public IActionResult UpdateCategory(CategoryModel categoryModel)     //for update category
        {
            var data = _adminService.UpdateCategory(categoryModel);
            return Ok(data);
        }
        [HttpPost]
        [Route("DaleteCategory")]
        public IActionResult DaleteCategory(CategoryModel categoryToDelete)   //for delete category
        {
            var data = _adminService.DeleteCategory(categoryToDelete);
            return Ok(data);
        }
        

        [HttpGet]
        [Route("GetProduct")]
        public IActionResult GetProducts()          //for get product
        {
            var data = _adminService.GetProducts();
            return Ok(data);
        }

        [HttpPost]
        [Route("SaveProduct")]

        public IActionResult SaveProduct(ProductModel newProduct)
        {
            int nextProductId = _adminService.GetNewProductId();
            newProduct.ImageUrl = @"Images/" + nextProductId + ".png";
            var path = $"{env.WebRootPath}\\Images\\{nextProductId + ".png"}";
            var fs = System.IO.File.Create(path);
            fs.Write(newProduct.FileContent, 0, newProduct.FileContent.Length);
            fs.Close();
            string uploadsFolder = Path.Combine(env.WebRootPath, "Images");
            var data = _adminService.SaveProduct(newProduct);
            return Ok(data);
        }

        //[HttpPost]
        //[Route("DeleteProduct")]
        //public IActionResult Delete(ProductModel productToDelete)
        //{
        //    var data = _adminService.DeleteProduct(productToDelete);
        //    return Ok(data);
        //}

        [HttpGet]
        [Route("GetProductStock")]
        public IActionResult GetProductStock()
        {
            var data = _adminService.GetProductStock();
            return Ok(data);
        }

        [HttpPost]
        [Route("UpdateProductStock")]

        public IActionResult UpdateProductStock(StockModel stock)
        {
            var data = _adminService.UpdateProductStock(stock);
          
            return Ok(data);
        }


        [HttpPost]
        [Route("DeleteProductById")]
        public IActionResult DeleteProduct(int Id)
        {
            //ProductModel model = new ProductModel();
            var data = _adminService.DeleteProduct(Id);
            return Ok(data);
        }
    }
}
