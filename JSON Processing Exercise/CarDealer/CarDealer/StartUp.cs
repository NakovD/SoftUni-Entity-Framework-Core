using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using var db = new CarDealerContext();

            var result = GetSalesWithAppliedDiscount(db);

            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var data = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(data!);

            context.SaveChanges();

            return $"Successfully imported {data!.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var data = JsonConvert.DeserializeObject<List<Part>>(inputJson)!
                .Where(p => p.SupplierId <= 31);

            context.Parts.AddRange(data!);

            context.SaveChanges();

            return $"Successfully imported {data!.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = config.CreateMapper();

            var carsDtos = JsonConvert.DeserializeObject<List<CarDto>>(inputJson);

            foreach (var carDto in carsDtos!)
            {
                var car = mapper.Map<Car>(carDto);
                context.Add(car);

                context.SaveChanges();

                var carParts = new List<PartCar>();

                foreach (var partId in carDto.PartsId)
                {
                    var carPart = new PartCar() { PartId = partId, CarId = car.Id };
                    carParts.Add(carPart);
                }

                car.PartsCars = carParts;

                context.SaveChanges();
            }

            return $"Successfully imported {carsDtos.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers!);

            context.SaveChanges();

            return $"Successfully imported {customers!.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales!);

            context.SaveChanges();

            return $"Successfully imported {sales!.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customersToExport = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    c.IsYoungDriver
                })
                .ToList();

            var json = JsonConvert.SerializeObject(customersToExport, Formatting.Indented);

            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsToExport = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance,
                });

            var json = JsonConvert.SerializeObject(carsToExport, Formatting.Indented);

            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TraveledDistance = c.TraveledDistance,
                    },
                    parts = c.PartsCars.Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("f2", new CultureInfo("en-US"))
                    })
                })
                .ToList();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    totalSpent = c.Sales.Select(s => s.Car.PartsCars.Sum(pc => pc.Part.Price))
                })
                .ToList()
                .Select(c => new
                {
                    c.fullName,
                    c.boughtCars,
                    spentMoney = c.totalSpent.Sum()
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars);

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount.ToString("f2"),
                    price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                    priceWithDiscount = (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - (s.Discount / 100))).ToString("f2")
                })
                .ToList();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;
        }
    }
}