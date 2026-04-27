namespace WebApi.Repositories;

public class AntifraudRepository : IAntifraudRepository
{
    private readonly AppDbContext _dbContext;

    public AntifraudRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AntifraudResult>> GetNearTransactionsAsync(
        ReadOnlyMemory<float> embedding, 
        CancellationToken cancellationToken)
    {
        return await _dbContext.AntifraudResults
            .AsNoTracking()
            .OrderBy(x => x.Embedding.CosineDistance(new Vector(embedding)))
            .Take(5)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsPopulatedAsync(CancellationToken cancellationToken)
    {
        var count = await _dbContext.AntifraudResults.CountAsync(cancellationToken);
        return count >= 100_000;
    }
}