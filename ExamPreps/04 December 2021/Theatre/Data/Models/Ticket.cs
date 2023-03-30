using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theatre.Data.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1.00, 100.00)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 10)]
        public sbyte RowNumber { get; set; }

        [ForeignKey(nameof(Play))]
        [Required]
        public int PlayId { get; set; }

        public Play Play { get; set; } = null!;

        [ForeignKey(nameof(Theatre))]
        [Required]
        public int TheatreId { get; set; }

        public Theatre Theatre { get; set; } = null!;
    }
}
