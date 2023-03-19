﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Trucks.DataProcessor.ImportDto
{
    public class ClientDto
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        public string Name { get; set; }

        [JsonProperty("Nationality")]
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Nationality { get; set; }

        [JsonProperty("Type")]
        [Required]
        public string Type { get; set; }

        [JsonProperty("Trucks")]
        public int[] Trucks { get; set; }
    }
}
