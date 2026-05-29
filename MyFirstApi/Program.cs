using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Models;

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

// CREATE: Add a new Shinobi
app.MapPost("/api/shinobis", async (AppDbContext db, Shinobi shinobi) =>
{
    db.Shinobis.Add(shinobi);
    await db.SaveChangesAsync();
    return Results.Created($"/api/shinobis/{shinobi.Id}", shinobi);
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
app.MapPut("/api/shinobis/{id}", async (int id, AppDbContext db, Shinobi updatedShinobi) =>
{
    var shinobi = await db.Shinobis.FindAsync(id);
    if (shinobi == null) return Results.NotFound();

    shinobi.Name = updatedShinobi.Name;
    shinobi.Rank = updatedShinobi.Rank;
    shinobi.PhotoUrl = updatedShinobi.PhotoUrl;
    shinobi.VillageId = updatedShinobi.VillageId;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE: Remove a Shinobi
app.MapDelete("/api/shinobis/{id}", async (int id, AppDbContext db) =>
{
    var shinobi = await db.Shinobis.FindAsync(id);
    if (shinobi == null) return Results.NotFound();

    db.Shinobis.Remove(shinobi);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// GET: All Villages
app.MapGet("/api/villages", async (AppDbContext db) => 
    await db.Villages.Include(v => v.Shinobis).ToListAsync());

// POST: Create Village
app.MapPost("/api/villages", async (Village village, AppDbContext db) => {
    db.Villages.Add(village);
    await db.SaveChangesAsync();
    return Results.Created($"/api/villages/{village.Id}", village);
});


// 1. Create a "Template" Jutsu (e.g., Rasengan)
app.MapPost("/api/jutsus", async (Jutsu jutsu, AppDbContext db) => {
    db.Jutsus.Add(jutsu);
    await db.SaveChangesAsync();
    return Results.Created($"/api/jutsus/{jutsu.Id}", jutsu);
});


app.MapGet("/api/jutsus", async (AppDbContext db) =>
{
    var jutsus = await db.Jutsus
        .Select(j => new 
        {
            j.Id,
            j.Name,
            j.Element
            // We keep it clean and avoid mapping the back-reference 
            // to Shinobis here to prevent another JSON loop!
        })
        .ToListAsync();

    return Results.Ok(jutsus);
});

// 2. The Relationship Route (The "Linker")
app.MapPost("/api/shinobis/{shinobiId}/add-jutsu/{jutsuId}", async (int shinobiId, int jutsuId, AppDbContext db) =>
{
    var shinobi = await db.Shinobis.Include(s => s.Jutsus).FirstOrDefaultAsync(s => s.Id == shinobiId);
    var jutsu = await db.Jutsus.FindAsync(jutsuId);

    if (shinobi == null || jutsu == null) return Results.NotFound("Shinobi or Jutsu not found.");

    if (!shinobi.Jutsus.Contains(jutsu))
    {
        shinobi.Jutsus.Add(jutsu);
        await db.SaveChangesAsync();
    }
    return Results.Ok($"{shinobi.Name} now knows {jutsu.Name}!");
});

// Microsoft.EntityFrameworkCore must be imported at the top of your file for .ToListAsync() to work


app.MapGet("/api/villages/name", async (AppDbContext db) =>
{
    //  Add .ToListAsync() right at the end of the query chain
    var villageData = await db.Villages
        .Select(v => new 
        {
            v.Id,
            v.Name,
            v.KageTitle
        })
        .ToListAsync(); // <-- This converts the IQueryable into a materializable Task!

    return Results.Ok(villageData);
});


app.Run();