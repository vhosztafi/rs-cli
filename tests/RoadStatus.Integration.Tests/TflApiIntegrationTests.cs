using System.Text.Json;
using RoadStatus.Core;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace RoadStatus.Integration.Tests;

public class TflApiIntegrationTests : IAsyncLifetime
{
    private WireMockServer? _wireMockServer;

    private static bool IsLiveIntegrationEnabled()
    {
        return Environment.GetEnvironmentVariable("RUN_LIVE_INTEGRATION") == "1";
    }

    private TflRoadStatusClient CreateClient()
    {
        if (IsLiveIntegrationEnabled())
        {
            var httpClient = new HttpClient();
            var appId = Environment.GetEnvironmentVariable("TFL_APP_ID");
            var appKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
            return new TflRoadStatusClient(httpClient, appId: appId, appKey: appKey);
        }
        else
        {
            return CreateMockedClient();
        }
    }

    private TflRoadStatusClient CreateMockedClient()
    {
        if (_wireMockServer == null)
        {
            throw new InvalidOperationException("WireMock server is not initialized. Ensure InitializeAsync has been called.");
        }

        var httpClient = new HttpClient();
        var baseUrl = _wireMockServer.Url!;
        return new TflRoadStatusClient(httpClient, baseUrl: baseUrl);
    }

    private static string GetMockedResponse()
    {
        var roadStatusDto = new
        {
            displayName = "A2",
            statusSeverity = "Good",
            statusSeverityDescription = "No Exceptional Delays"
        };
        return JsonSerializer.Serialize(new[] { roadStatusDto });
    }

    [Fact]
    public async Task GetRoadStatusAsync_A2_ReturnsRoadStatus()
    {
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
        var client = CreateClient();
        var roadId = RoadId.Parse("A233");

        if (!IsLiveIntegrationEnabled())
        {
            var exception = await Assert.ThrowsAsync<UnknownRoadException>(
                () => client.GetRoadStatusAsync(roadId));

            Assert.Equal("A233 is not a valid road", exception.Message);
        }
        else
        {
            var exception = await Assert.ThrowsAsync<UnknownRoadException>(
                () => client.GetRoadStatusAsync(roadId));

            Assert.Equal("A233 is not a valid road", exception.Message);
        }
    }

    public async Task InitializeAsync()
    {
        if (!IsLiveIntegrationEnabled())
        {
            _wireMockServer = WireMockServer.Start(new WireMockServerSettings
            {
                Urls = ["http://localhost:0"]
            });

            _wireMockServer
                .Given(Request.Create()
                    .WithPath("/Road/A2")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(GetMockedResponse()));

            _wireMockServer
                .Given(Request.Create()
                    .WithPath("/Road/A233")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(404));
        }

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_wireMockServer != null)
        {
            _wireMockServer.Stop();
            _wireMockServer.Dispose();
        }

        await Task.CompletedTask;
    }
}

