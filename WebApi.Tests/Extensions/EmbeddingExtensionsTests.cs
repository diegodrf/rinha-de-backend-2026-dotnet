using System.Text.Json;
using WebApi.DTOs;
using WebApi.Extensions;

namespace WebApi.Tests.Extensions;

public class EmbeddingExtensionsTests
{
    [Fact]
    public void ShouldCalculateThePredefinedEmbedding()
    {
        // Arrange
        var dto = JsonSerializer.Deserialize<TransactionRequestDto>("""
            {
            "id": "tx-3330991687",
            "transaction":      { "amount": 9505.97, "installments": 10, "requested_at": "2026-03-14T05:15:12Z" },
            "customer":         { "avg_amount": 81.28, "tx_count_24h": 20, "known_merchants": ["MERC-008", "MERC-007", "MERC-005"] },
            "merchant":         { "id": "MERC-068", "mcc": "7802", "avg_amount": 54.86 },
            "terminal":         { "is_online": false, "card_present": true, "km_from_home": 952.27 },
            "last_transaction": null
            }
            """);

        // Act
        var result = dto.ToEmbedding();

        // Assert
        Assert.Equal(0.9506f, result[0], 4);
        Assert.Equal(0.8333f, result[1], 4);
        Assert.Equal(1.0f, result[2], 4);
        Assert.Equal(0.2174f, result[3], 4);
        Assert.Equal(0.8333f, result[4], 4);
        Assert.Equal(-1f, result[5], 4);
        Assert.Equal(-1f, result[6], 4);
        Assert.Equal(0.9523f, result[7], 4);
        Assert.Equal(1.0f, result[8], 4);
        Assert.Equal(0f, result[9], 4);
        Assert.Equal(1f, result[10], 4);
        Assert.Equal(1f, result[11], 4);
        Assert.Equal(0.75f, result[12], 4);
        Assert.Equal(0.0055f, result[13], 4);
    }
}