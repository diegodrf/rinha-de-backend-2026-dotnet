namespace WebApi.Repositories;

public class AntifraudRepository : IAntifraudRepository
{
    private readonly AppDbContext _dbContext;

    public AntifraudRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static readonly Func<AppDbContext, Vector, IAsyncEnumerable<AntifraudResult>>
        GetNearTransactions = EF.CompileAsyncQuery((AppDbContext dbContext, Vector vector) => dbContext.AntifraudResults
            .AsNoTracking()
            .OrderBy(x => x.Embedding.CosineDistance(vector))
            .Take(5));
    private static readonly Func<AppDbContext, Task<int>>
        IsPopulated = EF.CompileAsyncQuery((AppDbContext dbContext) => dbContext.AntifraudResults
            .Count());

    public async Task<IReadOnlyCollection<AntifraudResult>> GetNearTransactionsAsync(
        ReadOnlyMemory<float> embedding, 
        CancellationToken cancellationToken)
    {
        var vector = new Vector(embedding);

        var array = new List<AntifraudResult>(5);
        await foreach (var t in GetNearTransactions(_dbContext, vector).WithCancellation(cancellationToken))
        {
            array.Add(t);
        }

        return array;
    }

    public async Task<bool> IsPopulatedAsync(CancellationToken cancellationToken)
    {
        var count = await IsPopulated(_dbContext);
        return count >= 100_000;
    }
}