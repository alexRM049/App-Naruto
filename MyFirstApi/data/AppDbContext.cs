using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models; // Needed so this file knows what a 'Shinobi' is

namespace MyFirstApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Shinobi> Shinobis { get; set; }
    public DbSet<Village> Villages { get; set; }
    public DbSet<Jutsu> Jutsus { get; set; }
}

