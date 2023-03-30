using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaporStore.Data.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [ForeignKey(nameof(Developer))]
        [Required]
        public int DeveloperId { get; set; }

        public Developer Developer { get; set; } = null!;

        [ForeignKey(nameof(Genre))]
        [Required]
        public int GenreId { get; set; }

        public Genre Genre { get; set; } = null!;

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        public ICollection<GameTag> GameTags { get; set; } = new List<GameTag>();
    }
}
