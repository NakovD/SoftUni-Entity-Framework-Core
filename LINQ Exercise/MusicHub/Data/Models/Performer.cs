namespace MusicHub.Data.Models;

using System.ComponentModel.DataAnnotations;

public class Performer
{

    public Performer()
    {
        PerformerSongs = new List<SongPerformer>();
    }

    public int Id { get; set; }

    [MaxLength(20)]
    public string FirstName { get; set; } = null!;

    [MaxLength(20)]

    public string LastName { get; set; } = null!;

    public int Age { get; set; }

    public decimal NetWorth { get; set; }

    public virtual ICollection<SongPerformer> PerformerSongs { get; set; }
}
