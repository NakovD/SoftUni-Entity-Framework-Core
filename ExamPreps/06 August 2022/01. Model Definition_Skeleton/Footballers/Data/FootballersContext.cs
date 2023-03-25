namespace Footballers.Data
{
    using Footballers.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class FootballersContext : DbContext
    {
        public DbSet<Footballer> Footballers { get; set; } = null!;

        public DbSet<Team> Teams { get; set; } = null!;

        public DbSet<Coach> Coaches { get; set; } = null!;

        public DbSet<TeamFootballer> TeamsFootballers { get; set; } = null!;

        public FootballersContext() { }

        public FootballersContext(DbContextOptions options)
            : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamFootballer>(entiity =>
            {
                entiity.HasKey(tf => new { tf.TeamId, tf.FootballerId });

                entiity
                .HasOne(tf => tf.Team)
                .WithMany(t => t.TeamsFootballers)
                .HasForeignKey(tf => tf.TeamId);

                entiity
                .HasOne(tf => tf.Footballer)
                .WithMany(f => f.TeamsFootballers)
                .HasForeignKey(tf => tf.FootballerId);
            });
        }
    }
}
