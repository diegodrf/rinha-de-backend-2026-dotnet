using System.Text.Json;
using WebApi;
using WebApi.DTOs;
using WebApi.Extensions;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(op =>
{
    var connectionString = builder.Configuration.GetConnectionString("rinha-db");
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

    op.UseNpgsql(connectionString, x => x.UseVector());
});

builder.Services.AddScoped<IAntifraudRepository, AntifraudRepository>();
builder.Services.AddScoped<IAntifraudService, AntifraudService>();

builder.Services.ConfigureHttpJsonOptions(op =>
{
    op.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await db.SeedAsync();
}

app.MapGet("/ready", async Task<IResult> (
    IAntifraudRepository repository,
    CancellationToken cancellationToken) =>
{
    var populated = await repository.IsPopulatedAsync(cancellationToken);
    if (!populated) TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable);

    for (var i = 0; i < 15; i++)
    {
        _ = await repository.GetNearTransactionsAsync(Utils.RequestExample.ToEmbedding(), cancellationToken);    
    }

    return TypedResults.Ok();
});

app.MapPost("/fraud-score", async Task<TransactionResponseDto> (
    TransactionRequestDto dto,
    IAntifraudService service,
    CancellationToken cancellationToken
    ) => await service.GetScoreAsync(dto, cancellationToken));

app.Run();