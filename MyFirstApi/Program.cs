using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Models;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Prevent JSON infinite loops with relationships
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAngular");

// 2. ROUTES (Must be AFTER builder.Build())

app.MapGet("/", () => "Naruto API is running!");

// POST: Register an archive user
app.MapPost("/api/auth/register", async (RegisterDto dto, AppDbContext db) =>
{
    var email = dto.Email.Trim().ToLowerInvariant();
    if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(email) || dto.Password.Length < 8)
    {
        return Results.BadRequest("Username, email, and a password of at least 8 characters are required.");
    }

    if (await db.Users.AnyAsync(user => user.Email == email))
    {
        return Results.Conflict("An account with that email already exists.");
    }

    var user = new AuthUser
    {
        Username = dto.Username.Trim(),
        Email = email,
        PasswordHash = HashPassword(dto.Password)
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/api/auth/users/{user.Id}", new { user.Id, user.Username, user.Email });
});

// POST: Authenticate an archive user
app.MapPost("/api/auth/login", async (LoginDto dto, AppDbContext db) =>
{
    var email = dto.Email.Trim().ToLowerInvariant();
    var user = await db.Users.SingleOrDefaultAsync(account => account.Email == email);

    if (user is null || !VerifyPassword(dto.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new
    {
        token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
        user = new { user.Id, user.Username, user.Email }
    });
});

// POST: Create Shinobi
app.MapPost("/api/shinobis", async (ShinobiCreateDto dto, AppDbContext db) => 
{
    // 1. Verify the assigned village exists
    var villageExists = await db.Villages.AnyAsync(v => v.Id == dto.VillageId);
    if (!villageExists) return Results.BadRequest("The assigned village does not exist.");

    // 2. Map the base DTO properties into a new tracking instance of your database entity
    var newShinobi = new Shinobi
    {
        Name = dto.Name,
        Rank = dto.Rank,
        PhotoUrl = dto.PhotoUrl,
        VillageId = dto.VillageId
    };

    // 3. Look up all Jutsus matching the IDs sent by the frontend tag array
    if (dto.JutsuIds != null && dto.JutsuIds.Any())
    {
        var assignedJutsus = await db.Jutsus
            .Where(j => dto.JutsuIds.Contains(j.Id))
            .ToListAsync();

        // Attach the real database entities straight to the cross-reference collection
        newShinobi.Jutsus = assignedJutsus;
    }

    // 4. Save to database (EF Core automatically populates your many-to-many join table!)
    db.Shinobis.Add(newShinobi);
    await db.SaveChangesAsync();

    return Results.Created($"/api/shinobis/{newShinobi.Id}", newShinobi);
});

// GET: All Shinobis
app.MapGet("/api/shinobis", async (AppDbContext db) =>
{
    var data = await db.Shinobis
        .Include(s => s.Village)
        .Include(s => s.Jutsus)
        .Select(s => new
        {
            s.Id,
            s.Name,
            s.Rank,
            s.PhotoUrl,
            s.VillageId,
            // Map only the specific village fields the frontend needs—break the loop!
            Village = s.Village != null ? new 
            { 
                s.Village.Id, 
                s.Village.Name, 
                s.Village.KageTitle 
            } : null,
            // Select only the clean Jutsu details
            Jutsus = s.Jutsus.Select(j => new 
            { 
                j.Id, 
                j.Name, 
                j.Element 
            }).ToList()
        })
        .ToListAsync();

    return Results.Ok(data);
});

// UPDATE: Modify an existing Shinobi's details
// FIX: Changed raw 'Shinobi' entity input parameter to 'ShinobiCreateDto' to allow updating Jutsus cleanly
app.MapPut("/api/shinobis/{id}", async (int id, ShinobiCreateDto dto, AppDbContext db) =>
{
    // FIX: Replaced FindAsync with FirstOrDefaultAsync + Include so we can manipulate the relational Jutsus tracking collection
    var shinobi = await db.Shinobis
        .Include(s => s.Jutsus)
        .FirstOrDefaultAsync(s => s.Id == id);
        
    if (shinobi == null) return Results.NotFound();

    // Verify village exists before assigning it
    var villageExists = await db.Villages.AnyAsync(v => v.Id == dto.VillageId);
    if (!villageExists) return Results.BadRequest("The assigned village does not exist.");

    shinobi.Name = dto.Name;
    shinobi.Rank = dto.Rank;
    shinobi.PhotoUrl = dto.PhotoUrl;
    shinobi.VillageId = dto.VillageId;

    // FIX: Update Jutsus mapping during an edit operation so user modifications save to the database
    shinobi.Jutsus.Clear();
    if (dto.JutsuIds != null && dto.JutsuIds.Any())
    {
        var assignedJutsus = await db.Jutsus
            .Where(j => dto.JutsuIds.Contains(j.Id))
            .ToListAsync();
        shinobi.Jutsus = assignedJutsus;
    }

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// GET: All Villages (Optimized for the Dropdown list)
// FIX: Replaced your old endpoint with this project select projection. 
// Old endpoint used .Include(v => v.Shinobis) which was leaking full data profiles down the wire.
app.MapGet("/api/villages", async (AppDbContext db) => 
{
    var villageData = await db.Villages
        .Select(v => new 
        {
            v.Id,
            v.Name,
            v.KageTitle
        })
        .ToListAsync();

    return Results.Ok(villageData);
});

// POST: Create Village
app.MapPost("/api/villages", async (Village village, AppDbContext db) => {
    db.Villages.Add(village);
    await db.SaveChangesAsync();
    return Results.Created($"/api/villages/{village.Id}", village);
});

// POST: Create a "Template" Jutsu (e.g., Rasengan)
app.MapPost("/api/jutsus", async (Jutsu jutsu, AppDbContext db) => {
    db.Jutsus.Add(jutsu);
    await db.SaveChangesAsync();
    return Results.Created($"/api/jutsus/{jutsu.Id}", jutsu);
});

// GET: All Jutsus (Optimized for the checkbox/tag layout view)
app.MapGet("/api/jutsus", async (AppDbContext db) =>
{
    var jutsus = await db.Jutsus
        .Select(j => new 
        {
            j.Id,
            j.Name,
            j.Element
        })
        .ToListAsync();

    return Results.Ok(jutsus);
});

// ==========================================
// DELETION ENDPOINTS
// ==========================================

// DELETE: Remove a Shinobi
// NOTE: Kept original single copy block, removed redundant duplicate copy from bottom of file
app.MapDelete("/api/shinobis/{id}", async (int id, AppDbContext db) =>
{
    var shinobi = await db.Shinobis.FindAsync(id);
    if (shinobi == null) return Results.NotFound();

    db.Shinobis.Remove(shinobi);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE: Remove a Jutsu
app.MapDelete("/api/jutsus/{id}", async (int id, AppDbContext db) =>
{
    var jutsu = await db.Jutsus
        .Include(j => j.Shinobis)
        .FirstOrDefaultAsync(j => j.Id == id);
        
    if (jutsu == null) return Results.NotFound();

    jutsu.Shinobis.Clear(); // Clear relational bridge table constraints
    db.Jutsus.Remove(jutsu);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE: Remove a Village (Includes Foreign Key guard validation checks)
app.MapDelete("/api/villages/{id}", async (int id, AppDbContext db) =>
{
    var village = await db.Villages.FindAsync(id);
    if (village == null) return Results.NotFound();

    var hasShinobis = await db.Shinobis.AnyAsync(s => s.VillageId == id);
    if (hasShinobis)
    {
        return Results.BadRequest("Cannot delete village. Move or delete its Shinobi population first.");
    }

    db.Villages.Remove(village);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run(); // <-- Server execution boundary

// ====================================================================
// DATA TRANSFER OBJECTS (DTOs) DEFINED BELOW SEPARATOR 
// ====================================================================

static string HashPassword(string password)
{
    var salt = RandomNumberGenerator.GetBytes(16);
    var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
    return $"100000.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
}

static bool VerifyPassword(string password, string storedHash)
{
    var parts = storedHash.Split('.');
    if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations)) return false;

    var salt = Convert.FromBase64String(parts[1]);
    var expectedHash = Convert.FromBase64String(parts[2]);
    var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
    return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
}

public class ShinobiCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Rank { get; set; } = "Genin";
    public string PhotoUrl { get; set; } = string.Empty;
    public int VillageId { get; set; }
    public List<int> JutsuIds { get; set; } = new();
}

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserFavoriteShinobi 
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ShinobiId { get; set; }
    public Shinobi Shinobi { get; set; } = null!;
}

