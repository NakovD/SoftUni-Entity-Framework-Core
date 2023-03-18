namespace ProductShop
{
    using Data;
    using Helpers;
    using DTOs.Import;
    using AutoMapper;
    using ProductShop.Models;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using ProductShop.DTOs.Export;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            using var context = new ProductShopContext();

            var result = GetProductsInRange(context);

            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var userDtos = helper.Deserialize<List<ImportUserDto>>(inputXml, "Users");

            var users = new List<User>();

            foreach (var userDto in userDtos)
            {
                var user = mapper.Map<User>(userDto);
                users.Add(user);
            }

            context.Users.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var productDtos = helper.Deserialize<List<ImportProductDto>>(inputXml, "Products");

            var products = new List<Product>();

            foreach (var productDto in productDtos)
            {
                var product = mapper.Map<Product>(productDto);
                products.Add(product);
            }

            context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var categoryDtos = helper.Deserialize<List<ImportCategoryDto>>(inputXml, "Categories");

            categoryDtos = categoryDtos.Where(c => c.Name != null).ToList();

            var categories = new List<Category>();

            foreach (var productDto in categoryDtos)
            {
                var product = mapper.Map<Category>(productDto);
                categories.Add(product);
            }

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var validCategoryIds = context.Categories.Select(c => c.Id).ToList();

            var validProductIds = context.Products.Select(p => p.Id).ToList();

            var productCategoryDtos = helper.Deserialize<List<ImportCategoryProductDto>>(inputXml, "CategoryProducts");

            productCategoryDtos = productCategoryDtos.Where(c => c.ProductId != null && c.CategoryId != null).ToList();

            var productsCategories = new List<CategoryProduct>();

            foreach (var productCategoryDto in productCategoryDtos)
            {
                if (!validCategoryIds.Contains((int)productCategoryDto.CategoryId!) || !validProductIds.Contains((int)productCategoryDto.ProductId!)) continue;
                var product = mapper.Map<CategoryProduct>(productCategoryDto);
                productsCategories.Add(product);
            }

            context.CategoryProducts.AddRange(productsCategories);

            context.SaveChanges();

            return $"Successfully imported {productsCategories.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var neededProducts = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ExportProductInRangeDto>(mapper.ConfigurationProvider)
                .ToList();

            var serializedData = helper.Serialize(neededProducts, "Products");

            return serializedData;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var dataToExport = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<ExportSoldProductsDto>(mapper.ConfigurationProvider)
                .ToList();

            var serializedData = helper.Serialize(dataToExport, "Users");

            return serializedData;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var dataToExport = context.Categories
                .ProjectTo<ExportCategoryDto>(mapper.ConfigurationProvider)
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            var serializedData = helper.Serialize(dataToExport, "Categories");

            return serializedData;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var allUsers = context.Users.Count();

            var dataToExport = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new ExportUserWithProductsDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsDto()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                        .Select(p => new ExportSoldProductDto()
                        {
                            Name = p.Name,
                            Price = p.Price,
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .Take(10)
                .ToArray();



            var withUsertsDto = new ExportUsersWithProductsDto
            {
                Count = context.Users.Count(u => u.ProductsSold.Count > 0),
                Users = dataToExport
            };

            var serializedData = helper.Serialize(withUsertsDto, "Users");

            return serializedData;
        }

        private static IMapper GetMapper()
        {
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            return mapper;
        }
    }
}