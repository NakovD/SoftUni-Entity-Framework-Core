using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType("User")]
    public class ExportSoldProductsDto
    {
        [XmlElement("firstName")]
        public string? FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; } = null!;

        [XmlArray("soldProducts")]
        public ExportSoldProductDto[] SoldProducts { get; set; } = null!;
    }


    [XmlType("Product")]
    public class ExportSoldProductDto
    {
        [XmlElement("name")]
        public string Name { get; set; } = null!;

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
