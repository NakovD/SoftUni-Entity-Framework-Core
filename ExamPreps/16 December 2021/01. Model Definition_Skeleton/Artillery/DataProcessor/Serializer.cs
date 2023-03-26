
namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ExportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            var dataToExport = context.Shells.
                AsNoTracking()
                .Where(s => s.ShellWeight > shellWeight)
                .Include(s => s.Guns)
                .ToList()
                .Select(s => new
                {
                    ShellWeight = s.ShellWeight,
                    Caliber = s.Caliber,
                    Guns = s.Guns
                    .Where(g => g.GunType == GunType.AntiAircraftGun)
                    .Select(g => new
                    {
                        GunType = g.GunType.ToString(),
                        GunWeight = g.GunWeight,
                        BarrelLength = g.BarrelLength,
                        Range = GetStringRange(g.Range)
                    })
                    .OrderByDescending(g => g.GunWeight)
                })
                .OrderBy(s => s.ShellWeight);


            var json = JsonConvert.SerializeObject(dataToExport, Formatting.Indented);

            return json;
        }

        private static string GetStringRange(int range)
        {
            if (range > 3000) return "Long-range";
            return "Regular range";
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {

            var dataToExport = context.Guns
                .AsNoTracking()
                .Where(g => g.Manufacturer.ManufacturerName == manufacturer)
                .OrderBy(g => g.BarrelLength)
                .Include(g => g.Manufacturer)
                .Include(g => g.CountriesGuns)
                .ThenInclude(cg => cg.Country)
                .ToList()
                .Select(g => new ExportGunDto()
                {
                    ManufacturerName = g.Manufacturer.ManufacturerName,
                    GunType = g.GunType.ToString(),
                    GunWeight = g.GunWeight,
                    BarrelLength = g.BarrelLength,
                    Range = g.Range,
                    Countries = g.CountriesGuns
                    .Where(cg => cg.Country.ArmySize > 4500000)
                    .OrderBy(cg => cg.Country.ArmySize)
                    .Select(cg => new ExportCountryDto
                    {
                        Country = cg.Country.CountryName,
                        ArmySize = cg.Country.ArmySize
                    }).ToList()
                })
                .ToList();

            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Guns");

            var serializer = new XmlSerializer(typeof(List<ExportGunDto>), root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            using var writer = new StringWriter(sb);

            serializer.Serialize(writer, dataToExport, namespaces);

            return sb.ToString().Trim();
        }
    }
}
