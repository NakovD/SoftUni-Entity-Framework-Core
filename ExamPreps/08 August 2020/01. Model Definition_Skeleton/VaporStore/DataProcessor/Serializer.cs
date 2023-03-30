namespace VaporStore.DataProcessor
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using VaporStore.DataProcessor.ExportDto;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var dataToExport = context.Genres
                .AsNoTracking()
                .Include(g => g.Games)
                 .ThenInclude(g => g.Developer)
                 .Include(g => g.Games)
                 .ThenInclude(g => g.Purchases)
                 .Include(g => g.Games)
                 .ThenInclude(g => g.GameTags)
                 .ThenInclude(gt => gt.Tag)
                .ToList()
                .Where(g => genreNames.Contains(g.Name))
                .Select(g => new
                {
                    g.Id,
                    Genre = g.Name,
                    Games = g.Games
                    .Where(ga => ga.Purchases.Any())
                    .Select(ga => new
                    {
                        ga.Id,
                        Title = ga.Name,
                        Developer = ga.Developer.Name,
                        Tags = string.Join(", ", ga.GameTags.Select(gt => gt.Tag.Name)),
                        Players = ga.Purchases.Count
                    })
                    .OrderByDescending(ga => ga.Players)
                    .ThenBy(ga => ga.Id),
                    TotalPlayers = g.Games.Sum(ga => ga.Purchases.Count)
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(dataToExport, Formatting.Indented);

            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string purchaseType)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var dataToExport = context.Users
                .AsNoTracking()
                .Include(u => u.Cards)
                .ThenInclude(c => c.Purchases)
                .ThenInclude(p => p.Game)
                .ThenInclude(g => g.Genre)
                .ToList()
                .Where(u => u.Cards.Select(c => c.Purchases).Any())
                .Select(u => new ExportUserDto()
                {
                    Username = u.Username,
                    Purchases = u.Cards
                    .SelectMany(c => c.Purchases)
                    .Where(p => p.Type.ToString() == purchaseType)
                    .Select(p => new ExportPurchaseDto()
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new ExportGameDto()
                        {
                            Title = p.Game.Name,
                            Genre = p.Game.Genre.Name,
                            Price = p.Game.Price == 0 ? "0" : p.Game.Price.ToString()
                        }
                    })
                    .OrderBy(p => p.Date)
                    .ToList(),
                    TotalSpent = u.Cards
                    .SelectMany(c => c.Purchases)
                    .Where(p => p.Type.ToString() == purchaseType)
                    .Sum(p => p.Game.Price).ToString()
                })
                .Select(u => new ExportUserDto
                {
                    Username = u.Username,
                    Purchases = u.Purchases,
                    TotalSpent = u.TotalSpent == "0.00" ? "0" : u.TotalSpent
                })
                .OrderByDescending(u => decimal.Parse(u.TotalSpent))
                .ThenBy(u => u.Username)
                .Where(u => u.Purchases.Count > 0)
                .ToList();

            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Users");

            var serializer = new XmlSerializer(typeof(List<ExportUserDto>), root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            using var writer = new StringWriter(sb);

            serializer.Serialize(writer, dataToExport, namespaces);

            return sb.ToString().Trim();
        }
    }
}