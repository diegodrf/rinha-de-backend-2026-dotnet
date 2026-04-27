using System.Text.Json;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AntifraudService> _loggerFake = Substitute.For<ILogger<AntifraudService>>();
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
            .Returns(_ => items.ToList());

        var service = new AntifraudService(repositoryFake, _loggerFake);

        // Act
        var result = await service.GetScoreAsync(dto, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Approved);
    }

    [Fact]
    public async Task ShouldReturnTrueOnlyAfterDatabaseHasShortDelay()
    {
        // Arrange
        var repositoryFake = Substitute.For<IAntifraudRepository>();
        repositoryFake.GetNearTransactionsAsync(Arg.Any<ReadOnlyMemory<float>>(), Arg.Any<CancellationToken>())
            .Returns([
                new AntifraudResult
                {
                    Id = 0, Embedding = new Vector(Utils.RequestExample.ToEmbedding()), Label = "legit"
                }
            ]);

        var sut = new AntifraudService(repositoryFake, _loggerFake);

        // Act
        var result = await sut.WarmUpAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ShouldReturnFalseWhenTheDatabaseHasLongDelayAndApplicationThrowsTimeout()
    {
        // Arrange
        var ctx = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        var repositoryFake = Substitute.For<IAntifraudRepository>();
        repositoryFake.GetNearTransactionsAsync(Arg.Any<ReadOnlyMemory<float>>(), Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200), ctx.Token);
                return
                [
                    new AntifraudResult
                    {
                        Id = 0,
                        Embedding = new Vector(Utils.RequestExample.ToEmbedding()),
                        Label = "legit"
                    }
                ];
            });

        var sut = new AntifraudService(repositoryFake, _loggerFake);

        // Act
        var result = await sut.WarmUpAsync(ctx.Token);

        // Assert
        Assert.False(result);
    }
}