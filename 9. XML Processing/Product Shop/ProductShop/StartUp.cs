using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XMLHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            ////1.
            //var usersXml = File.ReadAllText("../../../Datasets/users.xml");
            //var result = ImportUsers(context, usersXml);
            //System.Console.WriteLine(result);

            ////2.
            //var productsXml = File.ReadAllText("../../../Datasets/products.xml");
            //var result2 = ImportProducts(context, productsXml);
            //System.Console.WriteLine(result2);

            ////3.
            //var categoriesXml = File.ReadAllText("../../../Datasets/categories.xml");
            //var result3 = ImportCategories(context, categoriesXml);
            //System.Console.WriteLine(result3);

            //4.
            //var categoriesProductsXml = File.ReadAllText("../../../Datasets/categories-products.xml");
            //var result4 = ImportCategoryProducts(context, categoriesProductsXml);
            //System.Console.WriteLine(result4);

            //5.
            //var result5 = GetProductsInRange(context);
            //File.WriteAllText("../../../Datasets/Results/products-in-range.xml", result5);

            //6.
            //var result6 = GetSoldProducts(context);
            //File.WriteAllText("../../../Datasets/Results/users-sold-products.xml", result6);

            //7.
            //var result7 = GetCategoriesByProductsCount(context);
            //File.WriteAllText("../../../Datasets/Results/categories-by-products.xml", result7);

            //8.
            var result8 = GetUsersWithProducts(context);
            File.WriteAllText("../../../Datasets/Results/users-and-products.xml", result8);
        }

        //1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Users";

            var userResult = XMLConverter.Deserializer<ImportUserDto>(inputXml, rootElement);

            List<User> users = new List<User>();
            foreach (var dto in userResult)
            {
                User user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age
                };
                users.Add(user);
            }

            //or:
            //var users = userResult
            //    .Select(u => new User
            //    {
            //        FirstName = u.FirstName,
            //        LastName = u.LastName,
            //        Age = u.Age
            //    })
            //    .ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";

        }

        //2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Products";

            var productResult = XMLConverter.Deserializer<ImportProductDto>(inputXml, rootElement);

            List<Product> products = new List<Product>();
            foreach (var dto in productResult)
            {
                Product product = new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    SellerId = dto.SellerId,
                    BuyerId = dto.BuyerId
                };
                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Categories";

            var categoryResult = XMLConverter.Deserializer<ImportCategoryDto>(inputXml, rootElement);

            var categories = categoryResult
                .Where(c => c.Name != null)
                .Select(c => new Category
                {
                    Name = c.Name
                })
                .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        //4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            const string rootElement = "CategoryProducts";

            var cpResult = XMLConverter.Deserializer<ImportCategoryProductDto>(inputXml, rootElement);

            //var catProds = cpResult
            //    .Where(cp=>context.Categories.Any(c=>c.Id == cp.CategoryId) && context.Products.Any(p=>p.Id == cp.ProductId))
            //    .Select(cp => new CategoryProduct
            //    {
            //        CategoryId = cp.CategoryId,
            //        ProductId = cp.ProductId
            //    })
            //    .ToArray();

            List<CategoryProduct> catProds = new List<CategoryProduct>();
            foreach (var dto in cpResult)
            {
                if (context.Categories.Any(c => c.Id == dto.CategoryId) && context.Products.Any(p => p.Id == dto.ProductId))
                {
                    CategoryProduct catProd = new CategoryProduct
                    {
                        CategoryId = dto.CategoryId,
                        ProductId = dto.ProductId
                    };
                    catProds.Add(catProd);
                }
            }

            context.CategoryProducts.AddRange(catProds);
            context.SaveChanges();

            return $"Successfully imported {catProds.Count}";
        }

        //5. Export Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            const string rootElement = "Products";

            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ExportProductDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            var rersult = XMLConverter.Serialize(products, rootElement);

            return rersult;
        }

        //6. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            const string rootElement = "Users";

            var users = context
                .Users
                .Where(u => u.ProductsSold.Count > 0)
                .Select(u => new ExportUserWithSoldProductDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                                .Select(p => new ExportSoldProductDto
                                {
                                    Name = p.Name,
                                    Price = p.Price
                                })
                                .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToArray();

            var result = XMLConverter.Serialize(users, rootElement);
            return result;
        }

        //7. Export Categories by Sold Products
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            const string rootElement = "Categories";

            var categories = context
                .Categories
                .Select(c => new ExportCategoryDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(s => s.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(s => s.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            var result = XMLConverter.Serialize(categories, rootElement);

            return result;
        }

        //8. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            const string rootElement = "Users";

            var users = context
                .Users
                //.ToArray()
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u=>u.ProductsSold.Count)
                .Take(10)
                .Select(u => new ExportUserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportSoldProductsAndCountDto
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                                        .Where(ps => ps.Buyer != null)
                                        .Select(p => new ExportSoldProductDto
                                        {
                                            Name = p.Name,
                                            Price = p.Price
                                        })
                                        .OrderByDescending(p => p.Price)
                                        .ToArray()
                    }
                })
                .ToArray();

            var resultDto = new ExportUsersAndCount
            {
                Count = context.Users.Count(p=>p.ProductsSold.Any()),
                Users = users
            };

            var result = XMLConverter.Serialize(resultDto, rootElement);

            return result;

        }


    }
}