using System.Xml.Serialization;

namespace Artillery.DataProcessor.ExportDto
{
    [XmlType("Gun")]
    public class ExportGunDto
    {
        [XmlAttribute("Manufacturer")]
        public string ManufacturerName { get; set; } = null!;

        [XmlAttribute("GunType")]
        public string GunType { get; set; } = null!;

        [XmlAttribute("GunWeight")]
        public int GunWeight { get; set; }

        [XmlAttribute("BarrelLength")]
        public double BarrelLength { get; set; }

        [XmlAttribute("Range")]
        public int Range { get; set; }

        [XmlArray("Countries")]
        public List<ExportCountryDto> Countries { get; set; }
    }

    [XmlType("Country")]
    public class ExportCountryDto
    {
        [XmlAttribute("Country")]
        public string Country { get; set; } = null!;

        [XmlAttribute("ArmySize")]
        public int ArmySize { get; set; }
    }
}
