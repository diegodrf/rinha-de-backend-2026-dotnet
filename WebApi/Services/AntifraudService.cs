using System.Buffers;
using System.Diagnostics;
using WebApi.DTOs;
using WebApi.Extensions;
using WebApi.Repositories;

namespace WebApi.Services;

public class AntifraudService : IAntifraudService
{
    private const string Fraud = "fraud";
    private readonly IAntifraudRepository _antifraudRepository;
    private readonly ILogger<AntifraudService> _logger;

    public AntifraudService(IAntifraudRepository antifraudRepository, ILogger<AntifraudService> logger)
    {
        _antifraudRepository = antifraudRepository;
        _logger = logger;
    }

    public async Task<TransactionResponseDto> GetScoreAsync(
        TransactionRequestDto dto,
        CancellationToken cancellationToken)
    {
        var array = EmbeddingPool.Rent();

        var targets = await _antifraudRepository.GetNearTransactionsAsync(dto.ToEmbedding(array), cancellationToken);
        
        EmbeddingPool.Return(array);

        var frauds = 0;
        foreach (var t in targets)
        {
            if (t.Label.Equals(Fraud, StringComparison.OrdinalIgnoreCase))
            {
                frauds++;
            }
        }
        
        var fraudScore = (frauds / 5.0f);

        return new TransactionResponseDto
        {
            Approved = fraudScore < 0.6f,
            FraudScore = fraudScore
        };
    }

    public async Task<bool> WarmUpAsync(CancellationToken cancellationToken)
    {
        try
        {
            var array = EmbeddingPool.Rent();
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _ = await _antifraudRepository.GetNearTransactionsAsync(Utils.RequestExample.ToEmbedding(array),
                cancellationToken);
            stopwatch.Stop();
            
            EmbeddingPool.Return(array);

            _logger.LogInformation("Response time: {ResponseTime}", stopwatch.ElapsedMilliseconds);
            
            return stopwatch.ElapsedMilliseconds < Constants.DatabaseTargetResponseTimeInMilliseconds;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning("{Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            throw;
        }
    }
}