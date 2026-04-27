namespace WebApi.Entities;

public class AntifraudResult
{
    public long Id { get; init; }
    public required Vector Embedding { get; init; }
    public required string Label { get; init; }
}