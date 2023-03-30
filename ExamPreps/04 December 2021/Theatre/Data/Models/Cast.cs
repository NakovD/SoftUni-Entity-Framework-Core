using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theatre.Data.Models
{
    public class Cast
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string FullName { get; set; } = null!;

        [Required]
        public bool IsMainCharacter { get; set; }

        [Required]
        [RegularExpression("^\\+44-[\\d]{2}-[\\d]{3}-[\\d]{4}$")]
        public string PhoneNumber { get; set; } = null!;

        [ForeignKey(nameof(Play))]
        [Required]
        public int PlayId { get; set; }

        public Play Play { get; set; } = null!;
    }
}
