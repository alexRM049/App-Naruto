namespace MyFirstApi.Models;

public class Village
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KageTitle { get; set; } = string.Empty; // e.g. Hokage
    
    // Relationship: One Village has Many Shinobis
    public List<Shinobi> Shinobis { get; set; } = new();
} 