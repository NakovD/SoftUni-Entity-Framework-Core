namespace Theatre.DataProcessor
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var dataToExport = context.Theatres
                .AsNoTracking()
                .Where(t => t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count >= 20)
                .Select(t => new
                {
                    t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets
                    .Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5)
                    .Sum(ti => ti.Price),
                    Tickets = t.Tickets
                    .Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5)
                    .Select(ti => new
                    {
                        ti.Price,
                        ti.RowNumber
                    })
                    .OrderByDescending(ti => ti.Price)
                    .ToList()
                })
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                .ToList();

            var json = JsonConvert.SerializeObject(dataToExport, Formatting.Indented);

            return json;
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            var dataToExport = context.Plays
                .AsNoTracking()
                .Where(p => p.Rating <= raiting)
                .Include(p => p.Casts)
                .ToList()
                .Select(p => new ExportPlayDto()
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c"),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts
                    .Where(c => c.IsMainCharacter)
                    .Select(c => new ExportCastDto()
                    {
                        FullName = c.FullName,
                        MainCharacter = $"Plays main character in '{p.Title}'."
                    })
                    .OrderByDescending(c => c.FullName)
                    .ToList()
                })
                .OrderBy(p => p.Title)
                .ThenByDescending(p => p.Genre)
                .ToList();

            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Plays");

            var serializer = new XmlSerializer(typeof(List<ExportPlayDto>), root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            using var writer = new StringWriter(sb);

            serializer.Serialize(writer, dataToExport, namespaces);

            return sb.ToString().Trim();
        }
    }
}
