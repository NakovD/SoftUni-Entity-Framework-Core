using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("sale")]
    public class ExportSaleWithDiscountDto
    {
        [XmlElement("car")]
        public ExportSaleWithDiscountCarDto Car { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; } = null!;

        [XmlElement("price")]
        public string Price { get; set; }

        [XmlElement("price-with-discount")]
        public string PriceWithDiscount { get; set; }

        public class ExportSaleWithDiscountCarDto
        {
            [XmlAttribute("make")]
            public string Make { get; set; } = null!;

            [XmlAttribute("model")]
            public string Model { get; set; } = null!;

            [XmlAttribute("traveled-distance")]
            public long TraveledDistance { get; set; }
        }
    }
}
