using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Despatcher")]
    public class DespatcherDto
    {
        [XmlElement("Name")]
        [MinLength(2)]
        [MaxLength(41)]
        [Required]
        public string Name { get; set; } = null!;

        [XmlElement("Position")]
        public string? Position { get; set; }

        [XmlArray("Trucks")]
        public DespatcherTruckDto[] Trucks { get; set; }
    }


    [XmlType("Truck")]
    public class DespatcherTruckDto
    {
        [XmlElement("RegistrationNumber")]
        [RegularExpression(@"[A-Z]{2}[0-9]{4}[A-Z]{2}$")]
        [Required]
        public string RegistrationNumber { get; set; } = null!;

        [XmlElement("VinNumber")]
        [StringLength(17)]
        [Required]
        public string VinNumber { get; set; } = null!;

        [XmlElement("TankCapacity")]
        [Range(950, 1420)]
        public int? TankCapacity { get; set; }

        [XmlElement("CargoCapacity")]
        [Range(5000, 29000)]
        public int? CargoCapacity { get; set; }

        [XmlElement("CategoryType")]
        [Required]
        [Range(0, 3)]
        public int CategoryType { get; set; }

        [XmlElement("MakeType")]
        [Required]
        [Range(0, 4)]
        public int MakeType { get; set; }
    }
}
