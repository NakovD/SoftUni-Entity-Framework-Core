using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Footballers.Data.Models
{
    public class TeamFootballer
    {
        [ForeignKey(nameof(Team))]
        [Required]
        public int TeamId { get; set; }

        public Team Team { get; set; }

        [Required]
        [ForeignKey(nameof(Footballer))]
        public int FootballerId { get; set; }

        public Footballer Footballer { get; set; }
    }
}
