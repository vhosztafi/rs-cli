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

    [Fact]
    public void FormatJson_EmptyCollection_ReturnsEmptyArray()
    {
        var formatter = new RoadStatusFormatter();
        var roadStatuses = Array.Empty<CoreRoadStatus>();

        var result = formatter.FormatJson(roadStatuses);

        Assert.Equal("[]", result);
    }

    [Fact]
    public void FormatJson_SingleRoadStatus_ReturnsJsonArray()
    {
        var formatter = new RoadStatusFormatter();
        var roadStatuses = new[]
        {
            new CoreRoadStatus("A2", "Good", "No Exceptional Delays")
        };

        var result = formatter.FormatJson(roadStatuses);

        Assert.StartsWith("[", result);
        Assert.EndsWith("]", result);
        Assert.Contains("\"displayName\":\"A2\"", result);
        Assert.Contains("\"statusSeverity\":\"Good\"", result);
        Assert.Contains("\"statusDescription\":\"No Exceptional Delays\"", result);
    }

    [Fact]
    public void FormatJson_MultipleRoadStatuses_ReturnsJsonArray()
    {
        var formatter = new RoadStatusFormatter();
        var roadStatuses = new[]
        {
            new CoreRoadStatus("A2", "Good", "No Exceptional Delays"),
            new CoreRoadStatus("A3", "Closure", "Road closed")
        };

        var result = formatter.FormatJson(roadStatuses);

        Assert.StartsWith("[", result);
        Assert.EndsWith("]", result);
        Assert.Contains("\"displayName\":\"A2\"", result);
        Assert.Contains("\"displayName\":\"A3\"", result);
        Assert.Contains("\"statusSeverity\":\"Good\"", result);
        Assert.Contains("\"statusSeverity\":\"Closure\"", result);
    }
}

