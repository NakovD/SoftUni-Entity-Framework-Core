using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace VaporStore.DataProcessor.ImportDto
{
    public class ImportCardDto
    {
        [Required]
        [RegularExpression("^\\d{4} \\d{4} \\d{4} \\d{4}$")]
        public string Number { get; set; } = null!;

        [JsonProperty("CVC")]
        [Required]
        [RegularExpression("^\\d{3}$")]
        public string Cvc { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;
    }
}
