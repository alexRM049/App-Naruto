namespace MyFirstApi.Models;

public class UserFavoriteShinobi 
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ShinobiId { get; set; }
    public Shinobi Shinobi { get; set; } = null!;
}