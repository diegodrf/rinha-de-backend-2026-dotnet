using System.Buffers;
using System.Text.Json;
using NSubstitute;
using Pgvector;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Extensions;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Tests.Services;

public class AntifraudServiceTests
{
    [Fact]
    public void ShouldReturnResponseInCamelCase()
    {
        // Arrange
        var response = new TransactionResponseDto();
        
        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        Assert.Matches("(fraud(_)score)", json);
    }

    [Fact]
    public async Task ShouldRunWithSuccess()
    {
        // Arrange
        var dto = new TransactionRequestDto();
        
        var items = Enumerable
            .Range(0, 5)
            .Select(i => new AntifraudResult
            {
                Id = i,
                Embedding = new Vector(dto.ToEmbedding()),
                Label = "legit"
            });
        
        var repositoryFake = Substitute.For<IAntifraudRepository>();
        repositoryFake.GetNearTransactionsAsync(Arg.Any<ReadOnlyMemory<float>>(), Arg.Any<CancellationToken>())
            .Returns(x => items.ToList());
        
        var service = new AntifraudService(repositoryFake);

        // Act
        var result = await service.GetScoreAsync(dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Approved);
    }
}

