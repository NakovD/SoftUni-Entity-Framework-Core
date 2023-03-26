﻿using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    public class Shell
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(2, 1_680)]
        public double ShellWeight { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Caliber { get; set; } = null!;

        public ICollection<Gun> Guns { get; set; } = new List<Gun>();
    }
}
