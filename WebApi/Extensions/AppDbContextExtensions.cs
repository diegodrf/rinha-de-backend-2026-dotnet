using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using WebApi.Entities;
using WebApi.Persistence;

namespace WebApi.Extensions;

public static class AppDbContextExtensions
{
    extension(AppDbContext db)
    {
        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await db.AntifraudResults.AnyAsync(cancellationToken)) return;
            
            var filePath = Path.Combine(AppContext.BaseDirectory, "Assets", "references.json");
            await using var openStream = File.OpenRead(filePath);

            var json = await JsonSerializer.DeserializeAsync<IEnumerable<JsonSchema>>(
                openStream, 
                cancellationToken: cancellationToken);
            ArgumentNullException.ThrowIfNull(json);
            
            foreach (var c in json.Chunk(500))
            {
                var entities = c.Select(x => new AntifraudResult
                {
                    Embedding = new Vector(x.Vector),
                    Label = x.Label
                });
                db.AntifraudResults.AddRange(entities);
                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

file class JsonSchema
{
    [JsonPropertyName("vector")]
    public required float[] Vector { get ; set; }
    [JsonPropertyName("label")]
    public required string Label { get ; set; }
}