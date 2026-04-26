namespace WebApi.Dtos;

public record struct TransactionResponseDto
{
    public bool Approved { get; init; }
    public float FraudScore { get; set; }
}