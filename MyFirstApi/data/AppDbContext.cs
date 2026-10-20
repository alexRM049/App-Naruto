using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models; // Needed so this file knows what a 'Shinobi' is

namespace MyFirstApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Shinobi> Shinobis { get; set; }
    public DbSet<Village> Villages { get; set; }
    public DbSet<Jutsu> Jutsus { get; set; }
    public DbSet<AuthUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthUser>()
            .HasIndex(user => user.Email)
            .IsUnique();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<UserFavoriteShinobi>()
        .HasKey(ufs => new { ufs.UserId, ufs.ShinobiId });

    modelBuilder.Entity<UserFavoriteShinobi>()
        .HasOne(ufs => ufs.User)
        .WithMany(u => u.FavoriteShinobis)
        .HasForeignKey(ufs => ufs.UserId);

    modelBuilder.Entity<UserFavoriteShinobi>()
        .HasOne(ufs => ufs.Shinobi)
        .WithMany(s => s.FavoriteShinobis)
        .HasForeignKey(ufs => ufs.ShinobiId);
}

}

