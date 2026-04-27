using WebApi.DTOs;
using WebApi.Extensions;
using WebApi.Repositories;

namespace WebApi.Services;

public class AntifraudService : IAntifraudService
{
    private readonly IAntifraudRepository _antifraudRepository;

    public AntifraudService(IAntifraudRepository antifraudRepository)
    {
        _antifraudRepository = antifraudRepository;
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
}