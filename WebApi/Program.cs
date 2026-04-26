using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using WebApi.Dtos;
using WebApi.Extensions;
using WebApi.Persistence;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(op =>
{
    var connectionString = builder.Configuration.GetConnectionString("rinha-db");
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

    op.UseNpgsql(connectionString, x => x.UseVector());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await db.SeedAsync();
}

app.MapGet("/ready", () => "Alive!");
app.MapPost("/fraud-score", async Task<TransactionResponseDto> (
    TransactionRequestDto dto,
    AppDbContext dbContext,
    CancellationToken cancellationToken
    ) =>
{
    var vector = new Vector(EmbeddingService.Embedding(dto));

    var targets = await dbContext.AntifraudResults
        .AsNoTracking()
        .OrderBy(x => x.Embedding.CosineDistance(vector))
        .Take(5)
        .ToListAsync(cancellationToken);

    var frauds = targets.Count(x => x.Label == "fraud");
    var fraudScore = (frauds / 5.0f);

    return new TransactionResponseDto
    {
        Approved = fraudScore < 0.6f,
        FraudScore = fraudScore
    };
});

app.Run();