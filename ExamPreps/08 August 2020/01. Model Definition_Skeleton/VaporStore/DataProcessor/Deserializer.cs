namespace VaporStore.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.ImportDto;

    public static class Deserializer
    {
        public const string ErrorMessage = "Invalid Data";

        public const string SuccessfullyImportedGame = "Added {0} ({1}) with {2} tags";

        public const string SuccessfullyImportedUser = "Imported {0} with {1} cards";

        public const string SuccessfullyImportedPurchase = "Imported {0} for {1}";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedData = JsonConvert.DeserializeObject<List<ImportGameDto>>(jsonString)!;

            var validEntities = new List<Game>();

            var addedDevelopers = new List<Developer>();
            var addedGenres = new List<Genre>();
            var addedTags = new List<Tag>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);
                var areTagsEmpty = dto.Tags.Count == 0;

                if (!isValid || areTagsEmpty)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var developer = addedDevelopers.SingleOrDefault(d => d.Name == dto.Developer);
                var genre = addedGenres.SingleOrDefault(g => g.Name == dto.Genre);

                var entity = new Game()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    ReleaseDate = DateTime.ParseExact(dto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Developer = developer == null ? new Developer() { Name = dto.Developer } : developer,
                    Genre = genre == null ? new Genre() { Name = dto.Genre } : genre,
                    GameTags = new List<GameTag>()
                };

                if (developer == null) addedDevelopers.Add(entity.Developer);
                if (genre == null) addedGenres.Add(entity.Genre);

                foreach (var childDto in dto.Tags)
                {
                    var tag = addedTags.SingleOrDefault(t => t.Name == childDto);
                    var child = new GameTag()
                    {
                        Tag = tag == null ? new Tag() { Name = childDto } : tag,
                        Game = entity
                    };
                    entity.GameTags.Add(child);
                    if (tag == null) addedTags.Add(child.Tag);
                }

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfullyImportedGame, entity.Name, entity.Genre.Name, entity.GameTags.Count));
            }

            context.Games.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedData = JsonConvert.DeserializeObject<List<ImportUserDto>>(jsonString)!;

            var validEntities = new List<User>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new User()
                {
                    FullName = dto.FullName,
                    Username = dto.Username,
                    Email = dto.Email,
                    Age = dto.Age,
                    Cards = new List<Card>()
                };

                foreach (var childDto in dto.Cards)
                {
                    var isChildDtoValid = IsValid(childDto);
                    var isTypeValid = Enum.TryParse<CardType>(childDto.Type, out var type);

                    if (!isChildDtoValid || !isTypeValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var child = new Card()
                    {
                        Number = childDto.Number,
                        Cvc = childDto.Cvc,
                        Type = type
                    };
                    entity.Cards.Add(child);
                }

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfullyImportedUser, entity.Username, entity.Cards.Count));
            }

            context.Users.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Purchases");

            var serializer = new XmlSerializer(typeof(List<ImportPurchaseDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportPurchaseDto>)serializer.Deserialize(reader)!;

            var validEntities = new List<Purchase>();

            var cards = context.Cards.Select(c => new { c.Number, c.Id, c.User.Username });
            var games = context.Games.Select(g => new { g.Name, g.Id });

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);
                var isTypeValid = Enum.TryParse<PurchaseType>(dto.Type, out var type);
                var isDateValid = DateTime.TryParseExact(dto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var date);

                if (!isValid || !isTypeValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var card = cards.SingleOrDefault(c => c.Number == dto.Card)!;

                var entity = new Purchase()
                {
                    Type = type,
                    ProductKey = dto.ProductKey,
                    Date = date,
                    CardId = card.Id,
                    GameId = games.SingleOrDefault(g => g.Name == dto.Title)!.Id,
                };

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfullyImportedPurchase, dto.Title, card.Username));
            }
            context.Purchases.AddRange(validEntities);
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