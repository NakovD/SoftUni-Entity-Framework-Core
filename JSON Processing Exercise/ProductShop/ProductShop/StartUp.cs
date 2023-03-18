using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.Models;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using var db = new ProductShopContext();

            var result = GetUsersWithProducts(db);

            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(inputJson);

            context.Users.AddRange(users!);

            context.SaveChanges();

            return $"Successfully imported {users!.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);

            context.Products.AddRange(products!);

            context.SaveChanges();

            return $"Successfully imported {products!.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {

            var categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(inputJson);

            categories = categories!.Where(c => c.Name != null);

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            context.CategoriesProducts.AddRange(categoriesProducts!);

            context.SaveChanges();

            return $"Successfully imported {categoriesProducts!.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());

            var productsToExport = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ProductDto>(config)
                .ToList();

            var json = JsonConvert.SerializeObject(productsToExport);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());

            var dataToExport = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold.Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                })
                .ToList();

            var json = JsonConvert.SerializeObject(dataToExport);

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductShopProfile>());

            var dataToExport = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .ProjectTo<CategoryDTO>(config)
                .ToArray();

            var json = JsonConvert.SerializeObject(dataToExport, Formatting.Indented);

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Count(p => p.Buyer != null),
                        products = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new {
                            name = p.Name,
                            price = p.Price
                        })
                    }
                })
                .OrderByDescending(u => u.soldProducts.count)
                .ToList();

            var json = JsonConvert.SerializeObject(new {usersCount = users.Count, users }, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return json;
        }
    }
}