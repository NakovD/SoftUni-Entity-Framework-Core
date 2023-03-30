using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theatre.Data.Models.Enums;

namespace Theatre.Data.Models
{
    public class Play
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Title { get; set; } = null!;

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        [Range(0.00, 10.00)]
        public float Rating { get; set; }

        [Required]
        [Range(0, 3)]
        public Genre Genre { get; set; }

        [Required]
        [MaxLength(700)]
        public string Description { get; set; } = null!;

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Screenwriter { get; set; } = null!;

        public ICollection<Cast> Casts { get; set; } = new List<Cast>();

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
