namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Student
{
    public Student()
    {
        Homeworks = new List<Homework>();
        StudentsCourses = new List<StudentCourse>();
    }

    [Key]
    public int StudentId { get; set; }

    [Unicode(true)]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Unicode(false)]
    [StringLength(10)]
    public string? PhoneNumber { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime RegisteredOn { get; set; }

    public DateTime? Birthday { get; set; }

    public ICollection<Homework> Homeworks { get; set; }

    public ICollection<StudentCourse> StudentsCourses { get; set; }
}
