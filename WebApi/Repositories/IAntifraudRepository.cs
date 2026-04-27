namespace WebApi.Repositories;

public interface IAntifraudRepository
{
    Task<IReadOnlyCollection<AntifraudResult>> GetNearTransactionsAsync(
        ReadOnlyMemory<float> embedding,
        CancellationToken cancellationToken);
}