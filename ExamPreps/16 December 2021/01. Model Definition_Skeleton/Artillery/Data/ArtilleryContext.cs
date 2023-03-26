namespace Artillery.Data
{
    using Artillery.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ArtilleryContext : DbContext
    {
        public DbSet<Country> Countries { get; set; } = null!;

        public DbSet<Manufacturer> Manufacturers { get; set; } = null!;

        public DbSet<Shell> Shells { get; set; } = null!;

        public DbSet<Gun> Guns { get; set; } = null!;

        public DbSet<CountryGun> CountriesGuns { get; set; } = null!;

        public ArtilleryContext() 
        { 
        }

        public ArtilleryContext(DbContextOptions options)
            : base(options) 
        { 
        }

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
            modelBuilder.Entity<CountryGun>(entity =>
            {
                entity.HasKey(cg => new { cg.CountryId, cg.GunId });

                entity.HasOne(cg => cg.Country)
                .WithMany(c => c.CountriesGuns)
                .HasForeignKey(cg => cg.CountryId);

                entity.HasOne(cg => cg.Gun)
                .WithMany(cg => cg.CountriesGuns)
                .HasForeignKey(cg => cg.GunId);
            });
        }
    }
}
