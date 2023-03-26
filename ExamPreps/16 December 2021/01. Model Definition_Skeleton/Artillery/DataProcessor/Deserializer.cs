namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Countries");

            var serializer = new XmlSerializer(typeof(List<ImportCountryDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportCountryDto>)serializer.Deserialize(reader)!;

            var validCountries = new List<Country>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var country = new Country()
                {
                    CountryName = dto.Name,
                    ArmySize = dto.ArmySize
                };

                validCountries.Add(country);

                sb.AppendLine(string.Format(SuccessfulImportCountry, country.CountryName, country.ArmySize));
            }

            context.Countries.AddRange(validCountries);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Manufacturers");

            var serializer = new XmlSerializer(typeof(List<ImportManufacturerDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportManufacturerDto>)serializer.Deserialize(reader)!;

            var validEntities = new List<Manufacturer>();

            var uniqueManufacturers = new List<string>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);
                var doesManufacturerNameExist = uniqueManufacturers.Contains(dto.Name);

                if (!isValid || doesManufacturerNameExist)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Manufacturer()
                {
                    ManufacturerName = dto.Name,
                    Founded = dto.Founded
                };

                validEntities.Add(entity);
                uniqueManufacturers.Add(entity.ManufacturerName);

                var outputFounded = string.Join(", ", entity.Founded.Split(", ").TakeLast(2));

                sb.AppendLine(string.Format(SuccessfulImportManufacturer, entity.ManufacturerName, outputFounded));
            }

            context.Manufacturers.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Shells");

            var serializer = new XmlSerializer(typeof(List<ImportShellDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportShellDto>)serializer.Deserialize(reader)!;

            var validEntities = new List<Shell>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Shell()
                {
                    ShellWeight = dto.ShellWeight,
                    Caliber = dto.Caliber
                };

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfulImportShell, entity.Caliber, entity.ShellWeight));
            }

            context.Shells.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var gunDtos = JsonConvert.DeserializeObject<List<ImportGunDto>>(jsonString)!;

            var validGuns = new List<Gun>();

            var counter = 0;

            foreach (var gunDto in gunDtos)
            {
                var isValid = IsValid(gunDto);
                var isEnumValid = Enum.TryParse<GunType>(gunDto.GunType, out var gunType);

                if (!isValid || !isEnumValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var gun = new Gun()
                {
                    ManufacturerId = gunDto.ManufacturerId,
                    GunWeight = gunDto.GunWeight,
                    BarrelLength = gunDto.BarrelLength,
                    NumberBuild = gunDto.NumberBuild,
                    Range = gunDto.Range,
                    GunType = gunType,
                    ShellId = gunDto.ShellId,
                    CountriesGuns = new List<CountryGun>()
                };


                foreach (var countryId in gunDto.Countries)
                {
                    var cg = new CountryGun()
                    {
                        CountryId = countryId.Id,
                        Gun = gun
                    };
                    gun.CountriesGuns.Add(cg);
                    counter++;
                }

                validGuns.Add(gun);
                sb.AppendLine(string.Format(SuccessfulImportGun, gun.GunType.ToString(), gun.GunWeight, gun.BarrelLength));
            }

            context.AddRange(validGuns);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}