using System.Text.Json.Serialization;

namespace WebApi.DTOs;

public record struct TransactionResponseDto
{
    [JsonPropertyName("approved")] public bool Approved { get; init; } 
    [JsonPropertyName("fraud_score")] public float FraudScore { get; init; }
}