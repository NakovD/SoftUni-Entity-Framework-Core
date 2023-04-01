namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Creators");

            var serializer = new XmlSerializer(typeof(List<ImportCreatorDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportCreatorDto>)serializer.Deserialize(reader)!;

            var validEntities = new List<Creator>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Creator()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Boardgames = new List<Boardgame>()
                };

                foreach (var childDto in dto.Boardgames)
                {
                    var isChildDtoValid = IsValid(childDto);
                    if (!isChildDtoValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var childEntity = new Boardgame()
                    {
                        Name = childDto.Name,
                        Rating = childDto.Rating,
                        YearPublished = childDto.YearPublished,
                        CategoryType = (CategoryType)childDto.CategoryType,
                        Mechanics = childDto.Mechanics
                    };

                    entity.Boardgames.Add(childEntity);
                }

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfullyImportedCreator, entity.FirstName, entity.LastName, entity.Boardgames.Count));
            }

            context.Creators.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedData = JsonConvert.DeserializeObject<List<ImportSellerDto>>(jsonString)!;

            var validEntities = new List<Seller>();

            var boardgamesIds = context.Boardgames
                .AsNoTracking()
                .Select(b => b.Id)
                .ToList();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Seller()
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Country = dto.Country,
                    Website = dto.Website,
                    BoardgamesSellers = new List<BoardgameSeller>()
                };

                dto.Boardgames = dto.Boardgames.Distinct().ToList();

                foreach (var id in dto.Boardgames)
                {
                    var isChildValid = boardgamesIds.Contains(id);

                    if (!isChildValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var child = new BoardgameSeller()
                    {
                        BoardgameId = id,
                        Seller = entity
                    };

                    entity.BoardgamesSellers.Add(child);
                }

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfullyImportedSeller, entity.Name, entity.BoardgamesSellers.Count));
            }

            context.Sellers.AddRange(validEntities);
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
