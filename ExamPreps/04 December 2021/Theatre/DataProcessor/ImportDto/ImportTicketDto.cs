using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theatre.Data.Models;

namespace Theatre.DataProcessor.ImportDto
{
    public class ImportTicketDto
    {
        [JsonProperty("Price")]
        [Required]
        [Range(1.00, 100.00)]
        public decimal Price { get; set; }

        [JsonProperty("RowNumber")]
        [Required]
        [Range(1, 10)]
        public sbyte RowNumber { get; set; }

        [JsonProperty("PlayId")]
        [ForeignKey(nameof(Play))]
        [Required]
        public int PlayId { get; set; }
    }
}
