namespace P02_FootballBetting.Data.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;

public class Game
{

    public Game()
    {
        Bets = new List<Bet>();
        PlayersStatistics = new List<PlayerStatistic>();
    }

    public int GameId { get; set; }

    [ForeignKey(nameof(HomeTeam))]
    public int HomeTeamId { get; set; }

    public virtual Team HomeTeam { get; set; }

    [ForeignKey(nameof(AwayTeam))]
    public int AwayTeamId { get; set; }

    public virtual Team AwayTeam { get; set; }

    public int HomeTeamGoals { get; set; }

    public int AwayTeamGoals { get; set; }

    public DateTime DateTime { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal HomeTeamBetRate { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal AwayTeamBetRate { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal DrawBetRate { get; set; }

    public int Result { get; set; }

    public ICollection<Bet> Bets { get; set; }

    public ICollection<PlayerStatistic> PlayersStatistics { get; set; }
}
