using RoadStatus.Core;
using Xunit;

namespace RoadStatus.Core.Tests;

public class UnknownRoadExceptionTests
{
    [Fact]
    public void Message_FormattedCorrectly()
    {
        const string id = "A233";
        var exception = new UnknownRoadException(id);

        Assert.Equal("A233 is not a valid road", exception.Message);
    }
}
