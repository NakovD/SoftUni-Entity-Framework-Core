using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamDto
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression("[A-Za-z0-9 .-]+")]
        public string? Name { get; set; }

        [JsonProperty("Nationality")]
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string? Nationality { get; set; }

        [JsonProperty("Trophies")]
        [Required]
        [Range(1, int.MaxValue)]
        public int Trophies { get; set; }

        [JsonProperty("Footballers")]
        public List<int> Footballers { get; set; } = new List<int>();
    }
}
