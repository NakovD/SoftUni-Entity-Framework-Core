using System.ComponentModel.DataAnnotations;

namespace TeisterMask.Data.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression("^[A-Za-z\\d]+$")]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression("^\\d{3}-\\d{3}-\\d{4}$")]
        public string Phone { get; set; } = null!;

        public ICollection<EmployeeTask> EmployeesTasks { get; set; } = new List<EmployeeTask>();
    }
}
