using System.Text.Json;
using Microsoft.AspNetCore.Http.Timeouts;
using WebApi;
using WebApi.DTOs;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(op =>
{
    var connectionString = builder.Configuration.GetConnectionString("rinha-db");
    ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

    op.UseNpgsql(connectionString, x => x.UseVector());
});

builder.Services.AddScoped<IAntifraudRepository, AntifraudRepository>();
builder.Services.AddScoped<IAntifraudService, AntifraudService>();
builder.Services.AddRequestTimeouts(c =>
{
    c.AddPolicy("Policy", new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromMilliseconds(Constants.DatabaseTargetResponseTimeInMilliseconds),
        TimeoutStatusCode = StatusCodes.Status200OK,
        WriteTimeoutResponse = async (context) =>
        {
            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(new TransactionResponseDto() { Approved = false, FraudScore = 1f });
            await context.Response.WriteAsync(json);
        }
    });
});

builder.Services.ConfigureHttpJsonOptions(op =>
{
    op.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();
app.UseRequestTimeouts();

app.MapGet("/ready", async Task<IResult> (
        IAntifraudService service,
        CancellationToken cancellationToken) =>
{
    if (WU.Warmup) return TypedResults.Ok();
    
    var success = await service.WarmUpAsync(cancellationToken);

    if (!success) return TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable);

    WU.Warmup = true;
    return TypedResults.Ok();
    });

app.MapPost("/fraud-score", async Task<TransactionResponseDto> (
    TransactionRequestDto dto,
    IAntifraudService service,
    CancellationToken cancellationToken
    ) => await service.GetScoreAsync(dto, cancellationToken))
    .WithRequestTimeout("Policy");

app.Run();

public static class WU
{
    public static bool Warmup = false;
}