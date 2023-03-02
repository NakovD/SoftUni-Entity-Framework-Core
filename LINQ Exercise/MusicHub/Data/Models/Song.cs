namespace MusicHub.Data.Models;

using Enums;
using System.ComponentModel.DataAnnotations.Schema;

public class Song
{

    public Song()
    {
        SongPerformers = new List<SongPerformer>();
    }

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public TimeSpan Duration { get; set; }

    public DateTime CreatedOn { get; set; }

    public Genre Genre { get; set; }

    [ForeignKey(nameof(Album))]
    public int? AlbumId { get; set; }

    public virtual Album? Album { get; set; }

    [ForeignKey(nameof(Writer))]
    public int WriterId { get; set; }

    public virtual Writer Writer { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<SongPerformer> SongPerformers { get; set; }
}
