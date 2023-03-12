using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile() 
        {
            CreateMap<Product, ProductDto>()
                .ForMember(x => x.Seller, y => y.MapFrom(s => $"{s.Seller.FirstName ?? ""} {s.Seller.LastName}"))
                .ForMember(x => x.Price, y => y.MapFrom(s => s.Price));

            CreateMap<Category, CategoryDTO>()
                .ForMember(x => x.Category, y => y.MapFrom(s => s.Name))
                .ForMember(x => x.ProductsCount, y => y.MapFrom(s => s.CategoriesProducts.Count))
                .ForMember(x => x.AveragePrice, y => y.MapFrom(s => decimal.Parse(s.CategoriesProducts.Average(cp => cp.Product.Price).ToString("f2"))))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(s => decimal.Parse(s.CategoriesProducts.Sum(cp => cp.Product.Price).ToString("f2"))));

        }
    }
}
