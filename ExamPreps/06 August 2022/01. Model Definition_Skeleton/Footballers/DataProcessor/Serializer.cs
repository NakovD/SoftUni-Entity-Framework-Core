namespace Footballers.DataProcessor
{
    using Data;
    using Footballers.DataProcessor.ExportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            var exportData = context.Coaches
                .AsNoTracking()
                .Where(c => c.Footballers.Count > 0)
                .OrderByDescending(c => c.Footballers.Count)
                .ThenBy(c => c.Name)
                .Include(c => c.Footballers)
                .ToList()
                .Select(c => new ExportCoachDto
                {
                    CoachName = c.Name,
                    FootballersCount = c.Footballers.Count,
                    Footballers = c.Footballers
                    .Select(f => new ExportFootballerDto()
                    {
                        Name = f.Name,
                        Position = f.PositionType.ToString()
                    })
                    .OrderBy(f => f.Name)
                    .ToList()
                })
                .ToList();

            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Coaches");

            var serializer = new XmlSerializer(typeof(List<ExportCoachDto>), root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add(string.Empty, string.Empty);

            using var writer = new StringWriter(sb);

            serializer.Serialize(writer, exportData, namespaces);

            return sb.ToString().Trim();
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var exportData = context.Teams
                .AsNoTracking()
                .Where(t => t.TeamsFootballers.Any(tf => tf.Footballer.ContractStartDate >= date))
                .Include(t => t.TeamsFootballers)
                .ThenInclude(tf => tf.Footballer)
                .OrderByDescending(t => t.TeamsFootballers.Count(tf => tf.Footballer.ContractStartDate >= date))
                .ThenBy(t => t.Name)
                .ToList()
                .Select(t => new
                {
                    Name = t.Name,
                    Footballers = t.TeamsFootballers
                    .Where(tf => tf.Footballer.ContractStartDate >= date)
                    .OrderByDescending(tf => tf.Footballer.ContractEndDate)
                    .ThenBy(tf => tf.Footballer.Name)
                    .Select(tf => new
                    {
                        FootballerName = tf.Footballer.Name,
                        ContractStartDate = tf.Footballer.ContractStartDate.ToString("d", CultureInfo.InvariantCulture),
                        ContractEndDate = tf.Footballer.ContractEndDate.ToString("d", CultureInfo.InvariantCulture),
                        BestSkillType = tf.Footballer.BestSkillType.ToString(),
                        PositionType = tf.Footballer.PositionType.ToString(),
                    })
                    .ToList()
                })
                .Take(5);

            var json = JsonConvert.SerializeObject(exportData, Formatting.Indented);

            return json;
        }
    }
}
