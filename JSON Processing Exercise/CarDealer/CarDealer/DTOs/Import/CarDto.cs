namespace CarDealer.DTOs.Import
{
    public class CarDto
    {
        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        public string TraveledDistance { get; set; } = null!;

        public ICollection<int> PartsId { get; set; } = new HashSet<int>();
    }
}
