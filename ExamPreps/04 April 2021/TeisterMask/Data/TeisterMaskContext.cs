namespace TeisterMask.Data
{
    using Microsoft.EntityFrameworkCore;
    using TeisterMask.Data.Models;

    public class TeisterMaskContext : DbContext
    {
        public DbSet<Project> Projects { get; set; } = null!;

        public DbSet<Task> Tasks { get; set; } = null!;

        public DbSet<Employee> Employees { get; set; } = null!;

        public DbSet<EmployeeTask> EmployeesTasks { get; set; } = null!;

        public TeisterMaskContext()
        {
        }

        public TeisterMaskContext(DbContextOptions options)
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
            modelBuilder.Entity<EmployeeTask>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.TaskId });


                entity.HasOne(et => et.Employee)
                .WithMany(e => e.EmployeesTasks)
                .HasForeignKey(e => e.EmployeeId);

                entity.HasOne(et => et.Task)
                .WithMany(t => t.EmployeesTasks)
                .HasForeignKey(t => t.TaskId);
            });
        }
    }
}