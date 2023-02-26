namespace P02_FootballBetting.Data;

using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data.Models;

public class FootballBettingContext : DbContext
{
    public DbSet<Bet> Bets { get; set; }

    public DbSet<Color> Colors { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Game> Games { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<PlayerStatistic> PlayersStatistics { get; set; }

    public DbSet<Position> Positions { get; set; }

    public DbSet<Team> Teams { get; set; }

    public DbSet<Town> Towns { get; set; }

    public DbSet<User> Users { get; set; }

    public FootballBettingContext() : base()
    {
    }

    public FootballBettingContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        optionsBuilder.UseSqlServer("Server=.;Database=FootballBookmaker;Integrated Security=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>()
            .HasOne(t => t.PrimaryKitColor)
            .WithMany(c => c.PrimaryKitTeams)
            .HasForeignKey(t => t.PrimaryKitColorId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.SecondaryKitColor)
            .WithMany(c => c.SecondaryKitTeams)
            .HasForeignKey(t => t.SecondaryKitColorId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.HomeTeam)
            .WithMany(t => t.HomeGames)
            .HasForeignKey(g => g.HomeTeamId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.AwayTeam)
            .WithMany(t => t.AwayGames)
            .HasForeignKey(g => g.AwayTeamId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<PlayerStatistic>()
             .HasKey(ps => new { ps.PlayerId, ps.GameId });

        modelBuilder.Entity<PlayerStatistic>()
            .HasOne(ps => ps.Player)
            .WithMany(p => p.PlayersStatistics)
            .HasForeignKey(ps => ps.PlayerId);

        modelBuilder.Entity<PlayerStatistic>()
            .HasOne(ps => ps.Game)
            .WithMany(g => g.PlayersStatistics)
            .HasForeignKey(ps => ps.GameId);
    }
}
