using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression("^\\d{4} \\d{4} \\d{4} \\d{4}$")]
        public string Number { get; set; } = null!;

        [Required]
        [RegularExpression("^\\d{3}$")]
        public string Cvc { get; set; } = null!;

        [Required]
        [Range(0, 1)]
        public CardType Type { get; set; }

        [ForeignKey(nameof(User))]
        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
