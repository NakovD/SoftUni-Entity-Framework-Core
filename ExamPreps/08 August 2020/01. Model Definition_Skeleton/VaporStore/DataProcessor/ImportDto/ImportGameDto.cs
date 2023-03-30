using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.ImportDto
{
    public class ImportGameDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string ReleaseDate { get; set; } = null!;

        [Required]
        public string Developer { get; set; } = null!;

        [Required]
        public string Genre { get; set; } = null!;

        [Required]
        public List<string> Tags { get; set; } = new List<string>();
    }
}
