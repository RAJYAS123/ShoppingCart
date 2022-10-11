using Shop.DataModels.CustomModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Logic.Services
{
    public interface IAdminService
    {

        ResponseModel AdminLogin(LoginModel loginModel);
        CategoryModel SaveCategory(CategoryModel categoryModel);
        List<CategoryModel> GetCategories();
        bool UpdateCategory(CategoryModel categoryModel);
        bool DeleteCategory(CategoryModel categoryModel);
        List<ProductModel> GetProducts();
        bool DeleteProduct(int Id);
        int GetNewProductId();
        ProductModel SaveProduct(ProductModel newProduct);
        List<StockModel> GetProductStock();
        bool UpdateProductStock(StockModel stockModel);

    }
}
