namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlTypes;
    using System.Diagnostics.Metrics;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Plays");

            var serializer = new XmlSerializer(typeof(List<ImportPlayDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportPlayDto>)serializer.Deserialize(reader)!;

            var validEntities = new List<Play>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);
                var isTimeSpanValid = TimeSpan.TryParse(dto.Duration, CultureInfo.InvariantCulture, out var duration);
                var isDurationLessThanhour = duration.Hours == 0;
                var isGenreValid = Enum.TryParse<Genre>(dto.Genre, out var genre);

                duration = new TimeSpan(duration.Hours, duration.Minutes, duration.Seconds);

                if (!isValid || !isTimeSpanValid || isDurationLessThanhour || !isGenreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Play()
                {
                    Title = dto.Title,
                    Duration = duration,
                    Rating = dto.Rating,
                    Genre = genre,
                    Description = dto.Description,
                    Screenwriter = dto.ScreenWriter,
                };

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfulImportPlay, entity.Title, entity.Genre, entity.Rating));
            }

            context.Plays.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Casts");

            var serializer = new XmlSerializer(typeof(List<ImportCastDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportCastDto>)serializer.Deserialize(reader)!;

            var validEntities = new List<Cast>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Cast()
                {
                    FullName = dto.FullName,
                    IsMainCharacter = bool.Parse(dto.IsMainCharacter),
                    PhoneNumber = dto.PhoneNumber,
                    PlayId = dto.PlayId
                };

                validEntities.Add(entity);

                var actorStatus = entity.IsMainCharacter ? "main" : "lesser";

                sb.AppendLine(string.Format(SuccessfulImportActor, entity.FullName, actorStatus));
            }
            context.Casts.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedData = JsonConvert.DeserializeObject<List<ImportTheatreDto>>(jsonString)!;

            var validEntities = new List<Theatre>();

            var counter = 0;

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Theatre()
                {
                    Name = dto.Name,
                    NumberOfHalls = dto.NumberOfHalls,
                    Director = dto.Director,
                    Tickets = new List<Ticket>()
                };

                foreach (var childDto in dto.Tickets)
                {
                    var isChildValid = IsValid(childDto);

                    if (!isChildValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var child = new Ticket()
                    {
                        Price = childDto.Price,
                        RowNumber = childDto.RowNumber,
                        PlayId = childDto.PlayId,
                        Theatre = entity
                    };
                    counter++;
                    entity.Tickets.Add(child);
                }

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfulImportTheatre, entity.Name, entity.Tickets.Count));
            }
            context.Theatres.AddRange(validEntities);
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
