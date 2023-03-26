using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    [Index(nameof(ManufacturerName), IsUnique = true)]
    public class Manufacturer
    {
        [Key]
        [Required]
        public int Id { get; set; }


        [Required]
        [MinLength(4)]
        [MaxLength(40)]
        public string ManufacturerName { get; set; } = null!;

        [Required]
        [MinLength(10)]
        [MaxLength(100)]
        public string Founded { get; set; } = null!;

        public ICollection<Gun> Guns { get; set; } = new List<Gun>();
    }
}
