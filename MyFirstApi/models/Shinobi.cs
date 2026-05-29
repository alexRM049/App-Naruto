namespace MyFirstApi.Models;

public class Shinobi
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public int VillageId { get; set; }
    public Village? Village { get; set; }
    // Many-to-Many: A list of techniques this Shinobi knows
    public List<Jutsu> Jutsus { get; set; } = new();

    public string Rank { get; set; } = "Genin";
}