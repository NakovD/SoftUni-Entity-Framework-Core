using System.ComponentModel.DataAnnotations;

namespace Trucks.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [StringLength(40, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [StringLength(40, MinimumLength = 2)]
        public string Nationality { get; set; } = null!;

        public string Type { get; set; } = null!;

        public virtual ICollection<ClientTruck> ClientsTrucks { get; set; } = new List<ClientTruck>();
    }
}
