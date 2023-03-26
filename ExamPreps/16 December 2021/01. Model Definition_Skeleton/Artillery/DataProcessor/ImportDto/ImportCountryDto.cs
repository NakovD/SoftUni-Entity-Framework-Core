using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Country")]
    public class ImportCountryDto
    {
        [XmlElement("CountryName")]
        [Required]
        [MinLength(4)]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [XmlElement("ArmySize")]
        [Required]
        [Range(50_000, 10_000_000)]
        public int ArmySize { get; set; }
    }
}
