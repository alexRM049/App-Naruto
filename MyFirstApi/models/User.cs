namespace MyFirstApi.Models;

public class User 
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];

    // Many-to-Many navigation property
    public ICollection<UserFavoriteShinobi> FavoriteShinobis { get; set; } = new List<UserFavoriteShinobi>();
}