namespace P02_FootballBetting.Data.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;

public class Bet
{
    public int BetId { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal Amount { get; set; }

    public string Prediction { get; set; } = null!;

    public DateTime DateTime { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public virtual User User { get; set; }

    [ForeignKey(nameof(Game))]
    public int GameId { get; set; }

    public virtual Game Game { get; set; }
}
