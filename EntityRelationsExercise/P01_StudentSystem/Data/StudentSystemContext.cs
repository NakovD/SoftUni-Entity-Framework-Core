namespace P01_StudentSystem.Data;

using Microsoft.EntityFrameworkCore;

using P01_StudentSystem.Data.Models;

public class StudentSystemContext : DbContext
{
    public DbSet<Student> Students { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Homework> Homeworks { get; set; }

    public DbSet<Resource> Resources { get; set; }

    public DbSet<StudentCourse> StudentsCourses { get; set; }

    public StudentSystemContext() : base()
    {
    }

    public StudentSystemContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=SoftUni;Integrated Security=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentCourse>()
            .HasKey(sc => new { sc.StudentId, sc.CourseId });

        modelBuilder.Entity<StudentCourse>()
            .HasOne(sc => sc.Student)
            .WithMany(s => s.StudentsCourses)
            .HasForeignKey(sc => sc.StudentId);

        modelBuilder.Entity<StudentCourse>()
            .HasOne(sc => sc.Course)
            .WithMany(c => c.StudentsCourses)
            .HasForeignKey(sc => sc.CourseId);
    }
}
