using System.Text.Json;
using WebApi;
using WebApi.DTOs;
using WebApi.Extensions;
using WebApi.Persistence.CompiledModels;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(op =>
{
    var connectionString = builder.Configuration.GetConnectionString("rinha-db");
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

    op.UseNpgsql(connectionString, x => x.UseVector())
        .UseModel(AppDbContextModel.Instance);
});

builder.Services.AddScoped<IAntifraudRepository, AntifraudRepository>();
builder.Services.AddScoped<IAntifraudService, AntifraudService>();
builder.Services.AddRequestTimeouts();

builder.Services.ConfigureHttpJsonOptions(op =>
{
    op.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    op.SerializerOptions.TypeInfoResolver = AppJsonContext.Default;
});

var app = builder.Build();

app.UseRequestTimeouts();

using var scope = app.Services.CreateScope();
await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.OpenConnectionAsync();
await scope.ServiceProvider.GetRequiredService<IAntifraudRepository>()
    .GetNearTransactionsAsync(Utils.RequestExample.ToEmbedding(), CancellationToken.None);

app.MapGet("/ready", async Task<IResult> (
        IAntifraudService service,
        CancellationToken cancellationToken) =>
    {
        var success = await service.WarmUpAsync(cancellationToken);

        if (!success) TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable);

        return TypedResults.Ok();
    })
    .WithRequestTimeout(TimeSpan.FromSeconds(3));

app.MapPost("/fraud-score", async Task<TransactionResponseDto> (
    TransactionRequestDto dto,
    IAntifraudService service,
    CancellationToken cancellationToken
    ) => await service.GetScoreAsync(dto, cancellationToken));

app.Run();