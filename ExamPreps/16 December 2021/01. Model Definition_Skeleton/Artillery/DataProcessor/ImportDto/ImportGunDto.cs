using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artillery.DataProcessor.ImportDto
{
    public class ImportGunDto
    {
        [JsonProperty("ManufacturerId")]
        [Required]
        public int ManufacturerId { get; set; }

        [JsonProperty("GunWeight")]
        [Required]
        [Range(100, 1_350_000)]
        public int GunWeight { get; set; }

        [JsonProperty("BarrelLength")]
        [Required]
        [Range(2.00, 35.00)]
        public double BarrelLength { get; set; }

        [JsonProperty("NumberBuild")]
        public int? NumberBuild { get; set; }

        [JsonProperty("Range")]
        [Required]
        [Range(1, 100_000)]
        public int Range { get; set; }

        [JsonProperty("GunType")]
        [Required]
        public string GunType { get; set; } = null!;

        [JsonProperty("ShellId")]
        [Required]
        public int ShellId { get; set; }

        [JsonProperty("Countries")]
        public List<ImportGunCountryDto> Countries { get; set; } = new List<ImportGunCountryDto>();
    }

    public class ImportGunCountryDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
    }
}
