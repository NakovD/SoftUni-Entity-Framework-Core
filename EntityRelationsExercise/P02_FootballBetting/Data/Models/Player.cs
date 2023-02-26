namespace P02_FootballBetting.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class Player
{
    public Player()
    {
        PlayersStatistics = new List<PlayerStatistic>();
    }

    public int PlayerId { get; set; }

    public string Name { get; set; } = null!;

    public int SquadNumber { get; set; }

    [ForeignKey(nameof(Team))]
    public int TeamId { get; set; }

    public virtual Team Team { get; set; }

    [ForeignKey(nameof(Position))]
    public int PositionId { get; set; }

    public virtual Position Position { get; set; }

    public bool IsInjured { get; set; }

    public ICollection<PlayerStatistic> PlayersStatistics { get; set; }
}
