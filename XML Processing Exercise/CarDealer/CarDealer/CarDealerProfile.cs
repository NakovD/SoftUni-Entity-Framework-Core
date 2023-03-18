using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Globalization;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportSupplierDto, Supplier>();
            CreateMap<ImportPartDto, Part>();

            CreateMap<ImportCustomerDto, Customer>()
                .ForMember(d => d.BirthDate, opt => opt.MapFrom(s => DateTime.Parse(s.Birthdate, CultureInfo.InvariantCulture)));

            CreateMap<ImportSaleDto, Sale>();

            CreateMap<Car, ExportCarWithDistance>();

            CreateMap<Car, ExportCarBMWDto>();

            CreateMap<Supplier, ExportSupplierDto>()
                .ForMember(d => d.PartsCount, opt => opt.MapFrom(s => s.Parts.Count));
        }
    }
}
