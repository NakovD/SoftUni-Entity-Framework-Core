namespace P02_FootballBetting.Data.Models;

public class Country
{

    public Country()
    {
        Towns = new List<Town>();    
    }

    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Town> Towns { get; set; }
}
