using System.Net;
using System.Text;
using System.Text.Json;
using RoadStatus.Core;
using Xunit;

namespace RoadStatus.Integration.Tests;

public class TflApiIntegrationTests
{
    private static bool IsLiveIntegrationEnabled()
    {
        return Environment.GetEnvironmentVariable("RUN_LIVE_INTEGRATION") == "1";
    }

    private static TflRoadStatusClient CreateClient()
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

    private static TflRoadStatusClient CreateMockedClient()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, GetMockedResponse());
        var httpClient = new HttpClient(handler);
        return new TflRoadStatusClient(httpClient);
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
            var notFoundHandler = new TestHttpMessageHandler(HttpStatusCode.NotFound, string.Empty);
            var notFoundHttpClient = new HttpClient(notFoundHandler);
            var notFoundClient = new TflRoadStatusClient(notFoundHttpClient);

            var exception = await Assert.ThrowsAsync<UnknownRoadException>(
                () => notFoundClient.GetRoadStatusAsync(roadId));

            Assert.Equal("A233 is not a valid road", exception.Message);
        }
        else
        {
            var exception = await Assert.ThrowsAsync<UnknownRoadException>(
                () => client.GetRoadStatusAsync(roadId));

            Assert.Equal("A233 is not a valid road", exception.Message);
        }
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public TestHttpMessageHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}

