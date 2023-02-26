﻿namespace P02_FootballBetting.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Team
{
    public Team()
    {
        Players = new List<Player>();
        HomeGames = new List<Game>();
        AwayGames = new List<Game>();
    }

    [Key]
    public int TeamId { get; set; }

    public string Name { get; set; } = null!;

    public string LogoUrl { get; set; } = null!;

    public string Initials { get; set; } = null!;

    [Column(TypeName = "decimal(38, 2)")]
    public decimal Budget { get; set; }

    [ForeignKey(nameof(PrimaryKitColor))]
    public int PrimaryKitColorId { get; set; }

    public virtual Color PrimaryKitColor { get; set; }

    [ForeignKey(nameof(SecondaryKitColor))]
    public int SecondaryKitColorId { get; set; }

    public virtual Color SecondaryKitColor { get; set; }

    [ForeignKey(nameof(Town))]
    public int TownId { get; set; }

    public virtual Town Town { get; set; }

    public ICollection<Player> Players { get; set; }

    [InverseProperty(nameof(Game.HomeTeam))]
    public ICollection<Game> HomeGames { get; set; }

    [InverseProperty(nameof(Game.AwayTeam))]
    public ICollection<Game> AwayGames { get; set; }
}
