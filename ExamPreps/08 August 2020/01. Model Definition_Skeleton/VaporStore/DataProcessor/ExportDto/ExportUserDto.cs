using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDto
{
    [XmlType("User")]
    public class ExportUserDto
    {
        [XmlAttribute("username")]
        public string Username { get; set; } = null!;

        [XmlArray("Purchases")]
        public List<ExportPurchaseDto> Purchases { get; set; } = null!;

        [XmlElement("TotalSpent")]
        public string TotalSpent { get; set; } = null!;
    }
}
