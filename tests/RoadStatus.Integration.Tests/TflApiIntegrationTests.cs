using RoadStatus.Core;
using Xunit;

namespace RoadStatus.Integration.Tests;

public class TflApiIntegrationTests
{
    private static void SkipIfNotEnabled()
    {
        if (Environment.GetEnvironmentVariable("RUN_LIVE_INTEGRATION") != "1")
        {
            return;
        }
    }

    private static TflRoadStatusClient CreateClient()
    {
        var httpClient = new HttpClient();
        var appId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var appKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        return new TflRoadStatusClient(httpClient, appId: appId, appKey: appKey);
    }

    [Fact]
    public async Task GetRoadStatusAsync_A2_ReturnsRoadStatus()
    {
        SkipIfNotEnabled();

        var client = CreateClient();
        var roadId = RoadId.Parse("A2");
        var result = await client.GetRoadStatusAsync(roadId);

        Assert.NotNull(result);
        Assert.Equal("A2", result.DisplayName);
        Assert.NotNull(result.StatusSeverity);
        Assert.NotNull(result.StatusDescription);
    }

    [Fact]
    public async Task GetRoadStatusAsync_A233_ThrowsUnknownRoadException()
    {
        SkipIfNotEnabled();

        var client = CreateClient();
        var roadId = RoadId.Parse("A233");

        var exception = await Assert.ThrowsAsync<UnknownRoadException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Equal("A233 is not a valid road", exception.Message);
    }
}

