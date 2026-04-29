namespace WebApi.Tests;

public class UtilsTests
{
    [Theory]
    [InlineData(1.21, 1)]
    [InlineData(1, 1)]
    [InlineData(.9, .9)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(.1, .1)]
    public void WhenValueIsOutOfRange_ShouldRoundToTheNearAllowed(float input, float expected)
    {
        // Act
        var value = Utils.Truncate(input);
        
        // Assert
        Assert.Equal(expected, value);
    }

    [Fact]
    public void ShouldCreateAnArrayWith14Lenght()
    {
        // Act
        var sut = EmbeddingPool.Rent();
        
        // Assert
        Assert.Equal(14, sut.Length);
    }
}