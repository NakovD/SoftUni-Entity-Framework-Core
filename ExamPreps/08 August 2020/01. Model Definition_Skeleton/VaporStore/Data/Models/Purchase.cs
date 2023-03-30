using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, 1)]
        public PurchaseType Type { get; set; }

        [Required]
        [RegularExpression("^[A-Z\\d]{4}-[A-Z\\d]{4}-[A-Z\\d]{4}$")]
        public string ProductKey { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(Card))]
        [Required]
        public int CardId { get; set; }

        public Card Card { get; set; } = null!;

        [ForeignKey(nameof(Game))]
        [Required]
        public int GameId { get; set; }

        public Game Game { get; set; } = null!;
    }
}
