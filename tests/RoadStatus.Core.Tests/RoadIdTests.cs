using RoadStatus.Core;
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
}

