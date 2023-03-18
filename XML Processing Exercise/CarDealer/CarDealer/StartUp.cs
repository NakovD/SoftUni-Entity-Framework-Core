using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using System;
using static CarDealer.DTOs.Export.ExportCarWithPartsDto;
using static CarDealer.DTOs.Export.ExportSaleWithDiscountDto;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using var context = new CarDealerContext();

            var result = GetTotalSalesByCustomer(context);

            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var dtos = helper.Deserialize<List<ImportSupplierDto>>(inputXml, "Suppliers");

            dtos = dtos.Where(dto => dto.Name != null).ToList();

            var entities = new List<Supplier>();

            foreach (var dto in dtos)
            {
                var entity = mapper.Map<Supplier>(dto);
                entities.Add(entity);
            }

            context.Suppliers.AddRange(entities);

            context.SaveChanges();

            return $"Successfully imported {entities.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var suppliers = context.Suppliers.Select(s => s.Id).ToList();

            var dtos = helper.Deserialize<List<ImportPartDto>>(inputXml, "Parts");

            var entities = new List<Part>();

            foreach (var dto in dtos)
            {
                if (!suppliers.Contains(dto.SupplierId)) continue;
                var entity = mapper.Map<Part>(dto);
                entities.Add(entity);
            }

            context.Parts.AddRange(entities);

            context.SaveChanges();

            return $"Successfully imported {entities.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var partIds = context.Parts.Select(s => s.Id).ToList();

            var dtos = helper.Deserialize<List<ImportCarDto>>(inputXml, "Cars");

            var entities = new List<Car>();

            foreach (var dto in dtos)
            {
                var addedPartsInCar = new List<int>();

                var entity = new Car()
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TraveledDistance = dto.TraveledDistance
                };

                var carParts = new List<PartCar>();

                foreach (var dtoPart in dto.Parts)
                {
                    if (!partIds.Contains(dtoPart.Id) || addedPartsInCar.Contains(dtoPart.Id)) continue;
                    var carPart = new PartCar() { PartId = dtoPart.Id };
                    carParts.Add(carPart);
                    addedPartsInCar.Add(dtoPart.Id);
                }

                entity.PartsCars = carParts;

                entities.Add(entity);
            }

            context.Cars.AddRange(entities);

            context.SaveChanges();

            return $"Successfully imported {entities.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var dtos = helper.Deserialize<List<ImportCustomerDto>>(inputXml, "Customers");

            var entities = new List<Customer>();

            foreach (var dto in dtos)
            {
                if (dto.Name == null || dto.Birthdate == null) continue;

                var entity = mapper.Map<Customer>(dto);
                entities.Add(entity);
            }

            context.Customers.AddRange(entities);

            context.SaveChanges();

            return $"Successfully imported {entities.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var carIds = context.Cars.Select(c => c.Id).ToList();

            var dtos = helper.Deserialize<List<ImportSaleDto>>(inputXml, "Sales");

            var entities = new List<Sale>();

            foreach (var dto in dtos)
            {
                if (!carIds.Contains(dto.CarId)) continue;
                var entity = mapper.Map<Sale>(dto);
                entities.Add(entity);
            }

            context.Sales.AddRange(entities);

            context.SaveChanges();

            return $"Successfully imported {entities.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var dataToExport = context.Cars
                .Where(c => c.TraveledDistance > 2_000_000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarWithDistance>(mapper.ConfigurationProvider)
                .ToList();

            return helper.Serialize(dataToExport, "cars");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var dataToExport = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportCarBMWDto>(mapper.ConfigurationProvider)
                .ToList();

            return helper.Serialize(dataToExport, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var mapper = GetMapper();

            var helper = new XMLHelper();

            var dataToExport = context.Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<ExportSupplierDto>(mapper.ConfigurationProvider)
                .ToList();

            return helper.Serialize(dataToExport, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var helper = new XMLHelper();

            var dataToExport = context.Cars
                .Select(c => new ExportCarWithPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars.Select(pc => new ExportCarPartDto()
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(p => p.Model)
                .Take(5)
                .ToList();

            return helper.Serialize(dataToExport, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var helper = new XMLHelper();

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var dataToExport = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new
                {
                    FullName = c.Name,
                    IsYoung = c.IsYoungDriver,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Select(s => s.Car.PartsCars.Sum(pc => pc.Part.Price))
                })
                .ToList()
                .Select(c => new ExportCustomerDto()
                {
                    FullName = c.FullName,
                    BoughtCars = c.BoughtCars,
                    SpentMoney = c.SpentMoney.Sum(s => c.IsYoung ? decimal.Truncate(s * (decimal)0.95 * 100) / 100 : s)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToList();

            return helper.Serialize(dataToExport, "customers");
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var helper = new XMLHelper();

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var dataToExport = context.Sales
                .Select(s => new ExportSaleWithDiscountDto()
                {
                    Car = new ExportSaleWithDiscountCarDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("G29"),
                    PriceWithDiscount = (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - (s.Discount / 100))).ToString("G29")
                })
                .ToList();

            return helper.Serialize(dataToExport, "sales");
        }

        private static IMapper GetMapper()
        {
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            return mapper;
        }

    }
}