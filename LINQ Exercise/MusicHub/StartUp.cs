namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Text;

    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            var result = ExportSongsAboveDuration(context, 260);

            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var sb = new StringBuilder();

            var albums = context.Albums
                .Where(a => a.ProducerId.HasValue && a.ProducerId.Value == producerId)
                .ToArray()
                .OrderByDescending(a => a.Price)
                .Select(a => new
                {
                    a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyy"),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        s.Name,
                        Price = s.Price.ToString("f2"),
                        WriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.Name)
                    .ThenBy(s => s.WriterName)
                    .ToList(),
                    TotalAlbumPrice = a.Price.ToString("f2")
                })
                .ToList();

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                for (int i = 1; i <= album.Songs.Count; i++)
                {
                    var song = album.Songs[i - 1];
                    sb.AppendLine($"---#{i}");
                    sb.AppendLine($"---SongName: {song.Name}");
                    sb.AppendLine($"---Price: {song.Price}");
                    sb.AppendLine($"---Writer: {song.WriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice}");
            }

            return sb.ToString().Trim();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var sb = new StringBuilder();

            var songs = context.Songs
                .ToList()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    Performers = s.SongPerformers
                    .Select(sp => new
                    {
                        FullName = sp.Performer.FirstName + " " + sp.Performer.LastName
                    })
                    .OrderBy(p => p.FullName)
                    .ToArray(),
                    WriterName = s.Writer.Name,
                    AlbumProducerName = s.Album!.Producer!.Name ?? "No producer",
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToArray();

            for (int i = 1; i <= songs.Length; i++)
            {
                var song = songs[i - 1];

                sb.AppendLine($"-Song #{i}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.WriterName}");

                foreach (var performer in song.Performers)
                {
                    sb.AppendLine($"---Performer: {performer.FullName}");
                }

                sb.AppendLine($"---AlbumProducer: {song.AlbumProducerName}");
                sb.AppendLine($"---Duration: {song.Duration}");

            }


            return sb.ToString().Trim();
        }
    }
}
