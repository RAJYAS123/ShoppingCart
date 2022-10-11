using Shop.DataModels.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Admin.Services
{
   public interface IAdminPanelService
    {
        public Task<ResponseModel> AdmidLogin(LoginModel loginModel);
        public Task<CategoryModel> SaveCategory(CategoryModel categoryModel);
        public Task<List<CategoryModel>> GetCategories();
        public Task<bool> UpdateCategory(CategoryModel categoryModel);
        public Task<bool> DaleteCategory(CategoryModel categoryToDelete);
        Task<List<ProductModel>> GetProducts();
        Task<bool> DeleteProduct(ProductModel productModel);
        Task<ProductModel> SaveProduct(ProductModel productModel);
        Task<List<StockModel>> GetProductStock();
        Task<bool> UpdateProductStock(StockModel stock);

    }
}
