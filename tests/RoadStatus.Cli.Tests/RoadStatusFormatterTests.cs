using RoadStatus.Cli;
using RoadStatus.Core;
using CoreRoadStatus = RoadStatus.Core.RoadStatus;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class RoadStatusFormatterTests
{
    [Fact]
    public void Format_ReturnsExactText()
    {
        var formatter = new RoadStatusFormatter();
        var roadStatus = new CoreRoadStatus(
            "A2",
            "Good",
            "No Exceptional Delays");

        var result = formatter.Format(roadStatus);

        var expected = "The status of the A2 is as follows\r\n        Road Status is Good\r\n        Road Status Description is No Exceptional Delays\r\n";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Format_WithDifferentValues_ReturnsExactText()
    {
        var formatter = new RoadStatusFormatter();
        var roadStatus = new CoreRoadStatus(
            "A205",
            "Closure",
            "Road closed due to incident");

        var result = formatter.Format(roadStatus);

        var expected = "The status of the A205 is as follows\r\n        Road Status is Closure\r\n        Road Status Description is Road closed due to incident\r\n";
        Assert.Equal(expected, result);
    }
}

