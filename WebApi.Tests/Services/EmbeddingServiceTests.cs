using System.Buffers;
using System.Text.Json;
using WebApi.Dtos;
using WebApi.Services;

namespace WebApi.Tests.Services;

public class EmbeddingServiceTests
{
    [Theory]
    [InlineData(1.21, 1)]
    [InlineData(1, 1)]
    [InlineData(.9, .9)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(.1, .1)]
    public void WhenValueIsOutOfRange_ShouldRoundToTheNearAllowed(decimal input, decimal expected)
    {
        // Act
        var value = EmbeddingService.Truncate(input);
        
        // Assert
        Assert.Equal(expected, value);
    }

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
        var pool = ArrayPool<decimal>.Shared;
        var array = pool.Rent(14);
        var result = EmbeddingService.Embedding(dto, array);
        
        // Assert
        Assert.Equal(0.9506m, result[0], 4);
        Assert.Equal(0.8333m, result[1], 4);
        Assert.Equal(1.0m, result[2], 4);
        Assert.Equal(0.2174m, result[3], 4);
        Assert.Equal(0.8333m, result[4], 4);
        Assert.Equal(-1m, result[5], 4);
        Assert.Equal(-1m, result[6], 4);
        Assert.Equal(0.9523m, result[7], 4);
        Assert.Equal(1.0m, result[8], 4);
        Assert.Equal(0m, result[9], 4);
        Assert.Equal(1m, result[10], 4);
        Assert.Equal(1m, result[11], 4);
        Assert.Equal(0.75m, result[12], 4);
        Assert.Equal(0.0055m, result[13], 4);
        
        pool.Return(array);
        
    }
}

