namespace MusicHub.Data.Models;

using System.ComponentModel.DataAnnotations;

public class Producer
{

    public Producer()
    {
        Albums = new List<Album>();
    }

    public int Id { get; set; }

    [MaxLength(30)]
    public string Name { get; set; } = null!;

    public string? Pseudonym { get; set; } = null!;

    public string? PhoneNumber { get; set; } = null!;

    public virtual ICollection<Album> Albums { get; set; }
}
