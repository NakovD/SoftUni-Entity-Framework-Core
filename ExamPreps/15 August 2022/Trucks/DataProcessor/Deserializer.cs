namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlTypes;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Despatchers");

            var serializer = new XmlSerializer(typeof(List<DespatcherDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<DespatcherDto>)serializer.Deserialize(reader)!;

            var validDespatchers = new List<Despatcher>();

            var trucks = 0;

            foreach (var dto in deserializedData)
            {
                var isDespatcherValid = IsValid(dto);
                var isPositionValid = !string.IsNullOrEmpty(dto.Position);
                if (!isDespatcherValid || !isPositionValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var despatcher = new Despatcher()
                {
                    Name = dto.Name,
                    Position = dto.Position,
                    Trucks = new List<Truck>()
                };

                foreach (var dtoTruck in dto.Trucks)
                {
                    var isValid = IsValid(dtoTruck);

                    if (!isValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var truck = new Truck()
                    {
                        RegistrationNumber = dtoTruck.RegistrationNumber,
                        VinNumber = dtoTruck.VinNumber,
                        TankCapacity = dtoTruck.TankCapacity,
                        CargoCapacity = dtoTruck.CargoCapacity,
                        CategoryType = (CategoryType)dtoTruck.CategoryType,
                        MakeType = (MakeType)dtoTruck.MakeType,
                        Despatcher = despatcher
                    };

                    despatcher.Trucks.Add(truck);
                    trucks++;
                }

                sb.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcher.Name, despatcher.Trucks.Count));

                validDespatchers.Add(despatcher);
            }

            context.AddRange(validDespatchers);

            context.SaveChanges();

            return sb.ToString().Trim();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var clientDtos = JsonConvert.DeserializeObject<List<ClientDto>>(jsonString)!;

            var truckIds = context.Trucks.Select(t => t.Id).ToList();

            var validClients = new List<Client>();

            var trucks = 0;

            foreach (var clientDto in clientDtos)
            {
                var isClientValid = IsValid(clientDto);
                var isTypeUsual = clientDto.Type == "usual";
                if (!isClientValid || isTypeUsual)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                clientDto.Trucks = clientDto.Trucks.Distinct().ToArray();

                var client = new Client()
                {
                    Name = clientDto.Name,
                    Nationality = clientDto.Nationality,
                    Type = clientDto.Type,
                    ClientsTrucks = new List<ClientTruck>()

                };

                foreach (var truckId in clientDto.Trucks)
                {
                    var doesExistInDb = truckIds.Contains(truckId);
                    if (!doesExistInDb)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var clientTruck = new ClientTruck()
                    {
                        Client = client,
                        TruckId = truckId
                    };

                    client.ClientsTrucks.Add(clientTruck);
                    trucks++;
                }

                sb.AppendLine(string.Format(SuccessfullyImportedClient, client.Name, client.ClientsTrucks.Count));
                validClients.Add(client);
            }
            Console.WriteLine(validClients.Count);
            Console.WriteLine(trucks);

            context.Clients.AddRange(validClients);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}