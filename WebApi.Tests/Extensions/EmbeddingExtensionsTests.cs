using WebApi.Extensions;

namespace WebApi.Tests.Extensions;

public class EmbeddingExtensionsTests
{
    [Fact]
    public void ShouldCalculateThePredefinedEmbedding()
    {
        // Arrange
        var dto = Utils.RequestExample;

        // Act
        var result = dto.ToEmbedding(new float[14]);

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