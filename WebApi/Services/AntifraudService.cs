using System.Diagnostics;
using WebApi.DTOs;
using WebApi.Extensions;
using WebApi.Repositories;

namespace WebApi.Services;

public class AntifraudService : IAntifraudService
{
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
        var targets = await _antifraudRepository.GetNearTransactionsAsync(dto.ToEmbedding(), cancellationToken);

        var frauds = targets.Count(x => x.Label == "fraud");
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
            var count = 0;
            var stopwatch = new Stopwatch();

            while (count < 5)
            {
                stopwatch.Start();
                _ = await _antifraudRepository.GetNearTransactionsAsync(Utils.RequestExample.ToEmbedding(),
                    cancellationToken);
                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds < Constants.DatabaseTargetResponseTimeInMilliseconds)
                {
                    count++;
                }

                stopwatch.Reset();
                await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
            }

            return true;
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