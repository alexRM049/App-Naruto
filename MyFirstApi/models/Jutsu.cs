namespace MyFirstApi.Models;

public class Jutsu
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Element { get; set; } = string.Empty;

    // Many-to-Many: A list of Shinobis who know this technique
    public List<Shinobi> Shinobis { get; set; } = new();
}