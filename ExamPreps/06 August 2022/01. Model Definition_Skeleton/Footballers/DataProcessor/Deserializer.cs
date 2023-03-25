namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Coaches");

            var serializer = new XmlSerializer(typeof(List<ImportCoachDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportCoachDto>)serializer.Deserialize(reader)!;

            var validCoaches = new List<Coach>();

            foreach (var dto in deserializedData)
            {
                var isCoachValid = IsValid(dto);
                var isCoachNationalityValid = !string.IsNullOrEmpty(dto.Nationality);

                if (!isCoachValid || !isCoachNationalityValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var coach = new Coach()
                {
                    Name = dto.Name!,
                    Nationality = dto.Nationality!,
                    Footballers = new List<Footballer>()
                };

                foreach (var footbalerDto in dto.Footballers)
                {
                    var isFootballerValid = IsValid(footbalerDto);

                    if (!isFootballerValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    try
                    {
                        var contractStartDate = DateTime.ParseExact(footbalerDto.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        var contractEndDate = DateTime.ParseExact(footbalerDto.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if (contractStartDate > contractEndDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        var footballer = new Footballer()
                        {
                            Name = footbalerDto.Name!,
                            ContractStartDate = contractStartDate,
                            ContractEndDate = contractEndDate,
                            PositionType = (PositionType)footbalerDto.PositionType,
                            BestSkillType = (BestSkillType)footbalerDto.BestSkillType,
                            Coach = coach
                        };
                        coach.Footballers.Add(footballer);
                    }
                    catch (Exception)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                }

                validCoaches.Add(coach);
                sb.AppendLine(string.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count));
            }

            context.AddRange(validCoaches);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var teamDtos = JsonConvert.DeserializeObject<List<ImportTeamDto>>(jsonString)!;

            var validTeams = new List<Team>();

            var footballerIds = context.Footballers
                .AsNoTracking()
                .Select(f => f.Id)
                .ToList();

            foreach (var dto in teamDtos)
            {
                var isTeamValid = IsValid(dto);

                if (!isTeamValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                dto.Footballers = dto.Footballers.Distinct().ToList();

                var team = new Team()
                {
                    Name = dto.Name!.Trim(),
                    Nationality = dto.Nationality!,
                    Trophies = dto.Trophies,
                    TeamsFootballers = new List<TeamFootballer>()
                };

                foreach (var footballerId in dto.Footballers)
                {
                    var doesExistInDb = footballerIds.Contains(footballerId);

                    if (!doesExistInDb)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var tf = new TeamFootballer()
                    {
                        FootballerId = footballerId,
                        Team = team,
                    };

                    team.TeamsFootballers.Add(tf);
                }

                validTeams.Add(team);
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count));
            }

            context.AddRange(validTeams);
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
