using WebApi.DTOs;

namespace WebApi.Services;

public interface IAntifraudService
{
    Task<TransactionResponseDto> GetScoreAsync(
        TransactionRequestDto dto,
        CancellationToken cancellationToken);

    Task<bool> WarmUpAsync(CancellationToken cancellationToken);
}