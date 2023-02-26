namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Course
{
    public Course()
    {
        Resources = new List<Resource>();
        Homeworks = new List<Homework>();
        StudentsCourses = new List<StudentCourse>();
    }

    [Key]
    public int CourseId { get; set; }

    [Unicode(true)]
    [MaxLength(80)]
    public string Name { get; set; } = null!;

    [Unicode(true)]
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(4, 2)")]
    public decimal Price { get; set; }

    public ICollection<Resource> Resources { get; set; }

    public ICollection<Homework> Homeworks { get; set; }

    public ICollection<StudentCourse> StudentsCourses { get; set; }
}
