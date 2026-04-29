using System.Text.Json;
using System.Threading.Channels;
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

for (var i = 0; i < 100; i++)
{
    builder.Services.AddHostedService<DataBackgroundService>();
}

builder.Services.AddScoped<IAntifraudRepository, AntifraudRepository>();
builder.Services.AddScoped<IAntifraudService, AntifraudService>();
builder.Services.AddRequestTimeouts(c =>
{
    var defaultValue = new TransactionResponseDto() { Approved = false, FraudScore = 1f };
    c.AddPolicy("Policy", new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromMilliseconds(500),
        TimeoutStatusCode = StatusCodes.Status200OK,
        WriteTimeoutResponse = async context =>
        {
            await context.Response.WriteAsJsonAsync(defaultValue);
        }
    });
});

builder.Services.ConfigureHttpJsonOptions(op =>
{
    op.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddSingleton(_ =>
{
    BoundedChannelOptions options = new(1000)
    {
        SingleReader = false,
        SingleWriter = false,
        AllowSynchronousContinuations = false,
        FullMode = BoundedChannelFullMode.Wait
    };
    
    var channel = Channel.CreateBounded<TransactionChannelTemplate>(options);
    return channel;
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

app.MapPost("/fraud-score", async Task<IResult> (
    TransactionRequestDto dto,
    Channel<TransactionChannelTemplate> channel,
    CancellationToken cancellationToken
) =>
{
    var callback = ChannelPool.Rent();

    try
    {
        while (!channel.Writer.TryWrite(new(dto, callback)))
        {
            await Task.Delay(TimeSpan.FromMilliseconds(2), cancellationToken);
        }

        while (await callback.Reader.WaitToReadAsync(cancellationToken))
        {
            if (callback.Reader.TryRead(out var item)) return TypedResults.Ok(item);
        }

        return TypedResults.StatusCode(StatusCodes.Status500InternalServerError);
    }
    finally
    {
        ChannelPool.Return(callback);
    }
});
    //.WithRequestTimeout("Policy");

app.Run();

public static class WU
{
    public static bool Warmup = false;
}

public record TransactionChannelTemplate(
    TransactionRequestDto dto,
    Channel<TransactionResponseDto> responseChannel);