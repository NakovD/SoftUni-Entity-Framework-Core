namespace P02_FootballBetting.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class Town
{

    public Town()
    {
        Teams = new List<Team>();
    }

    public int TownId { get; set; }

    public string Name { get; set; } = null!;

    [ForeignKey(nameof(Country))]
    public int CountryId { get; set; }

    public virtual Country Country { get; set; }

    public ICollection<Team> Teams { get; set; }
}
