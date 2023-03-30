using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Cast")]
    public class ImportCastDto
    {
        [XmlElement("FullName")]
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string FullName { get; set; } = null!;

        [XmlElement("IsMainCharacter")]
        [Required]
        public string IsMainCharacter { get; set; } = null!;

        [XmlElement("PhoneNumber")]
        [Required]
        [RegularExpression("^\\+44-[\\d]{2}-[\\d]{3}-[\\d]{4}$")]
        public string PhoneNumber { get; set; } = null!;

        [XmlElement("PlayId")]
        [Required]
        public int PlayId { get; set; }
    }
}
