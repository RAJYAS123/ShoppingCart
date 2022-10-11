using Shop.DataModels.CustomModels;
using Shop.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Logic.Services
{
   public class AdminService : IAdminService
    {
        private readonly ShoppingCartDbBlazorContext _dbContext = null;
        public AdminService(ShoppingCartDbBlazorContext appDbContext)
        {
            this._dbContext = appDbContext;
        }
        public ResponseModel AdminLogin(LoginModel loginModel)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                var userdata = _dbContext.AdminInfos.Where(x => x.Email == loginModel.EmailId).FirstOrDefault();
                if(userdata!=null)
                {
                    if(userdata.Password==loginModel.Password)
                    {
                        responseModel.Status = true;
                        responseModel.Message = Convert.ToString(userdata.Id) + "," + userdata.Name + "," + userdata.Email;
                    }
                    else
                    {
                        responseModel.Status = false;
                        responseModel.Message = "Your Password is incorrect";
                    }
                }
                else
                {
                    responseModel.Status = false;
                    responseModel.Message = "Email not registerd. please check your Email Id";
                }
            }
            catch(Exception ex)
            {
                responseModel.Status = false;
                responseModel.Message = "An Error has occured Please try again !";
            }
            //responseModel.Status = false;
            //responseModel.Message = "Incorrect Id/Password";
            return responseModel;
        }

        public CategoryModel SaveCategory(CategoryModel categoryModel)
        {
            try
            {
                Category _category = new Category();
                _category.Name = categoryModel.Name;
                _dbContext.Add(_category);
                _dbContext.SaveChanges();
                return categoryModel;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public List<CategoryModel> GetCategories()
        {
            List<CategoryModel> _categoryList = new List<CategoryModel>();
            var data  = _dbContext.Categories.ToList();
            foreach(var item in data)
            {
                CategoryModel categoryModel = new CategoryModel();
                categoryModel.Id = item.Id;
                categoryModel.Name = item.Name;
                _categoryList.Add(categoryModel);
            }
            return _categoryList;
        }

        public bool UpdateCategory(CategoryModel categoryModel)
        {
            bool flag = false;
            var _category = _dbContext.Categories.Where(x => x.Id == categoryModel.Id).First();
            if(_category != null)
            {
                _category.Name = categoryModel.Name;
                _dbContext.Categories.Update(_category);
                _dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }
        public bool DeleteCategory(CategoryModel categoryModel)
        {
            bool flag = false;
            var _category = _dbContext.Categories.Where(x => x.Id == categoryModel.Id).First();
            if (_category != null)
            {
                
                _dbContext.Categories.Remove(_category);
                _dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        public List<ProductModel> GetProducts()
        {
            List<ProductModel> _productList = new List<ProductModel>();
            List<Category> categoryData = _dbContext.Categories.ToList();
            List<Product> productData = _dbContext.Products.ToList();
            foreach (var item in productData)
            {
                ProductModel productModel = new ProductModel();
                productModel.Id = item.Id;
                productModel.Name = item.Name;
                productModel.Price = Convert.ToInt32(item.Price);
                productModel.Stock = item.Stock;
                productModel.ImageUrl = item.ImageUrl;
                productModel.CategoryId = item.CategoryId;
                productModel.CategoryName = categoryData.Where(x => x.Id == item.CategoryId).Select(x => x.Name).FirstOrDefault();
                _productList.Add(productModel);
            }
            return _productList;
        }
        public ProductModel SaveProduct(ProductModel newProduct)
        {
            try
            {
                Product _product = new Product();
                _product.Name = newProduct.Name;
                _product.Price = Convert.ToString(newProduct.Price);
                _product.ImageUrl = newProduct.ImageUrl;
                _product.CategoryId = newProduct.CategoryId;
                _product.Stock = newProduct.Stock;
                _dbContext.Add(_product);
                _dbContext.SaveChanges();
                return newProduct;
            }
            catch
            {
                throw;
            }
        }

        public int GetNewProductId()
        {
            try
            {
                int newProductId = _dbContext.Products.ToList().OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
                return newProductId;
            }
            catch
            {
                throw;
            }
        }
        public bool DeleteProduct(int Id)
        {
            bool flag = false;
            var _product = _dbContext.Products.FirstOrDefault(x => x.Id == Id);
            if (_product != null)
            {

                _dbContext.Products.Remove(_product);
                _dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        public List<StockModel> GetProductStock()
        {
            List<StockModel> productStock = new List<StockModel>();
            List<Category> categoryData = _dbContext.Categories.ToList();
            List<Product> productData = _dbContext.Products.ToList();

            foreach (var p in productData)
            {
                StockModel _stock = new StockModel();
                _stock.Id = p.Id;
                _stock.Name = p.Name;
                _stock.Stock = p.Stock;
                _stock.CategoryName = categoryData.Where(x => x.Id == p.CategoryId).Select(x => x.Name).FirstOrDefault();
                productStock.Add(_stock);
            }
            return productStock;
        }

        public bool UpdateProductStock(StockModel stockModel)
        {
            bool flag = false;
            var _product = _dbContext.Products.Where(x => x.Id == stockModel.Id).FirstOrDefault();
            if(_product != null)
            {

                _product.Stock = stockModel.Stock + stockModel.NewStock;
                _dbContext.Products.Update(_product);
                _dbContext.SaveChanges();
                flag = true;
            }
            return flag;
            
        }

    }

}
