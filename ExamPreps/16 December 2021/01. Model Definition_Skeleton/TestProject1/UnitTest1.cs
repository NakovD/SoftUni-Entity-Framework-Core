//Resharper disable InconsistentNaming, CheckNamespace

using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Artillery;
using Artillery.Data;

[TestFixture]
public class Import_000_002
{
    private IServiceProvider serviceProvider;

    private static readonly Assembly CurrentAssembly = typeof(StartUp).Assembly;

    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ArtilleryProfile>();
        });

        this.serviceProvider = ConfigureServices<ArtilleryContext>("Artillery");
    }

    [Test]
    public void ImportManufacturersZeroTest()
    {
        var context = this.serviceProvider.GetService<ArtilleryContext>();

        var inputXml =
            @"<?xml version='1.0' encoding='UTF-8'?>
<Manufacturers>
  <Manufacturer>
    <ManufacturerName>BAE Systems</ManufacturerName>
    <Founded>30 November 1999, London, England</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>BAE</ManufacturerName>
    <Founded>30 November 1999, London, England</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Aviation Industry Corporation of China</ManufacturerName>
    <Founded>April 1, 1951, Chaoyang District, Beijing, China</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>General Dynamics</ManufacturerName>
    <Founded>February 7, 1899, Reston, Virginia, United States</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>General Dynamics</ManufacturerName>
    <Founded>February 7, 1899, Reston, Virginia, United States</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Raytheon Technologies</ManufacturerName>
    <Founded>2020, Waltham, Massachusetts, United States</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Northrop Grumman</ManufacturerName>
    <Founded>1994, 2980 Fairview Park Drive, West Falls Church, Virginia, United States</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Lockheed Martin</ManufacturerName>
    <Founded>March 15, 1995, Bethesda, Maryland, United States</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Izhmekh</ManufacturerName>
    <Founded>July 20, 1942, Izhevsk, USSR</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>IzhmekhInvalid</ManufacturerName>
    <Founded>Izhev</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>IzhmekhInvalidManufacturerasdasdfsdfsdffsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfe</ManufacturerName>
    <Founded>Izhevsk, USSR July 20, 1942</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Škoda</ManufacturerName>
    <Founded>Plzeň 1859, Kingdom of Bohemia, Austrian Empire</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>BAE Systems</ManufacturerName>
    <Founded>30 November 1999, London, England</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Krupp</ManufacturerName>
    <Founded>1881, Essen, German Empire</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Obukhov State Plant</ManufacturerName>
    <Founded>1863, Saint Petersburg, Russia</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Obukhov Plant Invalid</ManufacturerName>
    <Founded>1863, Saint Petersburg, Russia IzhmekhInvalidManufacturerNameIzhmekhInvalidManufacturerNameIzhmekhInvalid</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Putilov plant</ManufacturerName>
    <Founded>February 28, 1801, Helsinki, Coast Gulf of Finland</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Schneider et Cie</ManufacturerName>
    <Founded>1836, Schneider et Cie, France</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Colt's Manufacturing Company</ManufacturerName>
    <Founded>1855, Hartford, Connecticut, United States</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Glock Ges.m.b.H.</ManufacturerName>
    <Founded>1942, Wien,  Austria</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Almaz-Antey</ManufacturerName>
    <Founded>2002, Moscow, Russia</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Thales Group</ManufacturerName>
    <Founded>6 December 2000, Paris, France</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Leonardo S.p.A.</ManufacturerName>
    <Founded>1948, Rome, Italy</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Raytheon Technologies</ManufacturerName>
    <Founded>1992, Boston, United States</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>L3Harris Technologies</ManufacturerName>
    <Founded>June 29, 2019, Melbourne, Florida, United States</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Norinco</ManufacturerName>
    <Founded>August 1988, Xicheng District, Beijing, China</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName></ManufacturerName>
    <Founded>March 1, 2002, Haidian District, Beijing, China</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>China Electronics Technology Group</ManufacturerName>
    <Founded>March 1, 2002, Haidian District, Beijing, China</Founded>
  </Manufacturer>
  <Manufacturer>
    <ManufacturerName>Putilov plant</ManufacturerName>
    <Founded>February 28, 1801, Helsinki, Coast Gulf of Finland</Founded>
  </Manufacturer>
</Manufacturers>";

        ;
        var actualOutput =
            Artillery.DataProcessor.Deserializer.ImportManufacturers(context, inputXml).TrimEnd();
        ;
        var expectedOutput = "Successfully import manufacturer BAE Systems founded in London, England.\r\nInvalid data.\r\nSuccessfully import manufacturer Aviation Industry Corporation of China founded in Beijing, China.\r\nSuccessfully import manufacturer General Dynamics founded in Virginia, United States.\r\nInvalid data.\r\nSuccessfully import manufacturer Raytheon Technologies founded in Massachusetts, United States.\r\nSuccessfully import manufacturer Northrop Grumman founded in Virginia, United States.\r\nSuccessfully import manufacturer Lockheed Martin founded in Maryland, United States.\r\nSuccessfully import manufacturer Izhmekh founded in Izhevsk, USSR.\r\nInvalid data.\r\nInvalid data.\r\nSuccessfully import manufacturer Škoda founded in Kingdom of Bohemia, Austrian Empire.\r\nInvalid data.\r\nSuccessfully import manufacturer Krupp founded in Essen, German Empire.\r\nSuccessfully import manufacturer Obukhov State Plant founded in Saint Petersburg, Russia.\r\nInvalid data.\r\nSuccessfully import manufacturer Putilov plant founded in Helsinki, Coast Gulf of Finland.\r\nSuccessfully import manufacturer Schneider et Cie founded in Schneider et Cie, France.\r\nSuccessfully import manufacturer Colt's Manufacturing Company founded in Connecticut, United States.\r\nSuccessfully import manufacturer Glock Ges.m.b.H. founded in Wien,  Austria.\r\nSuccessfully import manufacturer Almaz-Antey founded in Moscow, Russia.\r\nSuccessfully import manufacturer Thales Group founded in Paris, France.\r\nSuccessfully import manufacturer Leonardo S.p.A. founded in Rome, Italy.\r\nInvalid data.\r\nSuccessfully import manufacturer L3Harris Technologies founded in Florida, United States.\r\nSuccessfully import manufacturer Norinco founded in Beijing, China.\r\nInvalid data.\r\nSuccessfully import manufacturer China Electronics Technology Group founded in Beijing, China.\r\nInvalid data.";

        var assertContext = this.serviceProvider.GetService<ArtilleryContext>();

        const int expectedManufacturerCount = 20;
        var actualManufacturerCount = assertContext.Manufacturers.Count();

        Assert.That(actualManufacturerCount, Is.EqualTo(expectedManufacturerCount),
            $"Inserted {nameof(context.Manufacturers)} count is incorrect!");

        Assert.That(actualOutput, Is.EqualTo(expectedOutput).NoClip,
            $"{nameof(Artillery.DataProcessor.Deserializer.ImportManufacturers)} output is incorrect!");
    }

    private static Type GetType(string modelName)
    {
        var modelType = CurrentAssembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == modelName);

        Assert.IsNotNull(modelType, $"{modelName} model not found!");

        return modelType;
    }

    private static IServiceProvider ConfigureServices<TContext>(string databaseName)
        where TContext : DbContext
    {
        var services = ConfigureDbContext<TContext>(databaseName);

        var context = services.GetService<TContext>();

        try
        {
            context.Model.GetEntityTypes();
        }
        catch (InvalidOperationException ex) when (ex.Source == "Microsoft.EntityFrameworkCore.Proxies")
        {
            services = ConfigureDbContext<TContext>(databaseName, useLazyLoading: true);
        }

        return services;
    }

    private static IServiceProvider ConfigureDbContext<TContext>(string databaseName, bool useLazyLoading = false)
        where TContext : DbContext
    {
        var services = new ServiceCollection()
          .AddDbContext<TContext>(t => t
          .UseInMemoryDatabase(Guid.NewGuid().ToString())
          );

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }
}