using System.Net;
using System.Text;
using System.Text.Json;
using RoadStatus.Core;
using Xunit;

namespace RoadStatus.Core.Tests;

public class TflRoadStatusClientTests
{
    [Fact]
    public async Task GetRoadStatusAsync_HappyPath_ReturnsRoadStatus()
    {
        const string displayName = "A2";
        const string statusSeverity = "Good";
        const string statusDescription = "No Exceptional Delays";

        var roadStatusDto = new
        {
            displayName = displayName,
            statusSeverity = statusSeverity,
            statusSeverityDescription = statusDescription
        };
        var jsonResponse = JsonSerializer.Serialize(new[] { roadStatusDto });

        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, jsonResponse);
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient, "https://api.tfl.gov.uk");

        var roadId = RoadId.Parse("A2");
        var result = await client.GetRoadStatusAsync(roadId);

        Assert.Equal(displayName, result.DisplayName);
        Assert.Equal(statusSeverity, result.StatusSeverity);
        Assert.Equal(statusDescription, result.StatusDescription);
    }

    [Fact]
    public async Task GetRoadStatusAsync_NotFound_ThrowsUnknownRoadException()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.NotFound, string.Empty);
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient, "https://api.tfl.gov.uk");

        var roadId = RoadId.Parse("A233");

        var exception = await Assert.ThrowsAsync<UnknownRoadException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Equal("A233 is not a valid road", exception.Message);
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
