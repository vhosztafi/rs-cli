using Xunit;

namespace RoadStatus.Core.Tests;

public class RoadIdTests
{
    [Fact]
    public void ToString_ReturnsOriginalValue()
    {
        const string expected = "A2";
        var roadId = RoadId.Parse(expected);

        var result = roadId.ToString();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_NullValue_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => RoadId.Parse(null!));
        Assert.Contains("Road ID cannot be null or empty.", exception.Message);
        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void Parse_EmptyString_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => RoadId.Parse(string.Empty));
        Assert.Contains("Road ID cannot be null or empty.", exception.Message);
        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void Parse_Whitespace_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => RoadId.Parse("   "));
        Assert.Contains("Road ID cannot be null or empty.", exception.Message);
        Assert.Equal("value", exception.ParamName);
    }
}

