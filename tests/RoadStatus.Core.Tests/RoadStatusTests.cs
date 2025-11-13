using Xunit;

namespace RoadStatus.Core.Tests;

public class RoadStatusTests
{
    [Fact]
    public void ValueRetention_AllFieldsPreserved()
    {
        const string displayName = "A2";
        const string statusSeverity = "Good";
        const string statusDescription = "No Exceptional Delays";

        var roadStatus = new RoadStatus(displayName, statusSeverity, statusDescription);

        Assert.Equal(displayName, roadStatus.DisplayName);
        Assert.Equal(statusSeverity, roadStatus.StatusSeverity);
        Assert.Equal(statusDescription, roadStatus.StatusDescription);
    }
}
