using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    public class Country
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(40)]
        public string CountryName { get; set; } = null!;

        [Required]
        [Range(50_000, 10_000_000)]
        public int ArmySize { get; set; }

        public ICollection<CountryGun> CountriesGuns { get; set; } = new List<CountryGun>();
    }
}
