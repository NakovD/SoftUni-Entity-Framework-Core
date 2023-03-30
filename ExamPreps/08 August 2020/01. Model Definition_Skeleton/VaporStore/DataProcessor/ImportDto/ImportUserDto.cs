using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.ImportDto
{
    public class ImportUserDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; } = null!;

        [Required]
        [RegularExpression("^[A-Z]{1}[a-z]+ [A-Z]{1}[a-z]+$")]
        public string FullName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [Range(3, 103)]
        public int Age { get; set; }

        public List<ImportCardDto> Cards { get; set; } = new List<ImportCardDto>();
    }
}
