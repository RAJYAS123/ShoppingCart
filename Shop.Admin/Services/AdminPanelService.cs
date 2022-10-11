using Shop.DataModels.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shop.Admin.Services
{
    public class AdminPanelService : IAdminPanelService
    {
        private readonly HttpClient httpClient;
        public AdminPanelService(HttpClient _httpClient)
        {
            this.httpClient = _httpClient;
        }

        public async Task<ResponseModel> AdmidLogin(LoginModel loginModel)
        {
            return await httpClient.PostJsonAsync<ResponseModel>("api/admin/AdminLogin", loginModel);
        }
        public async Task<CategoryModel> SaveCategory(CategoryModel categoryModel)
        {
            return await httpClient.PostJsonAsync<CategoryModel>("api/admin/SaveCategory", categoryModel);
        }
        public async Task<List<CategoryModel>> GetCategories()
        {
            return await httpClient.GetJsonAsync<List<CategoryModel>>("api/admin/GetCategories");
        }
        public async Task<bool> UpdateCategory(CategoryModel categoryModel)
        {
            return await httpClient.PostJsonAsync<bool>("api/admin/UpdateCategory", categoryModel);
        }
        public async Task<bool> DaleteCategory(CategoryModel categoryToDelete)
        {
            return await httpClient.PostJsonAsync<bool>("api/admin/DaleteCategory", categoryToDelete);
        }
        public async Task<List<ProductModel>> GetProducts()
        {
            return await httpClient.GetJsonAsync<List<ProductModel>>("api/admin/GetProduct");
            //return await httpClient.GetJsonAsync<List<ProductModel>>("api/admin/GetProducts")
        }
        public async Task<bool> DeleteProduct(ProductModel productModel) 
        {
            //var Id = productToDelete.Id;
            return await httpClient.PostJsonAsync<bool>("api/admin/DeleteProductById", productModel);
        }
        public async Task<ProductModel> SaveProduct(ProductModel productModel)
        {
            return await httpClient.PostJsonAsync<ProductModel>("api/admin/SaveProduct", productModel);
        }
        public async Task<List<StockModel>> GetProductStock()
        {
            return await httpClient.GetJsonAsync<List<StockModel>>("api/admin/GetProductStock");
        }
        public async Task<bool> UpdateProductStock(StockModel stock)
        {
            return await httpClient.PostJsonAsync<bool>("api/admin/UpdateProductStock", stock);
        }
    }
}
