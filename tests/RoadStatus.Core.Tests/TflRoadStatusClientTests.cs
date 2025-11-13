using System.Net;
using System.Text;
using System.Text.Json;
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
            displayName,
            statusSeverity,
            statusSeverityDescription = statusDescription
        };
        var jsonResponse = JsonSerializer.Serialize(new[] { roadStatusDto });

        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, jsonResponse);
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

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
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A233");

        var exception = await Assert.ThrowsAsync<UnknownRoadException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Equal("A233 is not a valid road", exception.Message);
    }

    [Fact]
    public async Task GetRoadStatusAsync_PascalCaseJson_ReturnsRoadStatus()
    {
        const string displayName = "A2";
        const string statusSeverity = "Good";
        const string statusDescription = "No Exceptional Delays";

        var roadStatusDto = new
        {
            DisplayName = displayName,
            StatusSeverity = statusSeverity,
            StatusSeverityDescription = statusDescription
        };
        var jsonResponse = JsonSerializer.Serialize(new[] { roadStatusDto });

        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, jsonResponse);
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");
        var result = await client.GetRoadStatusAsync(roadId);

        Assert.Equal(displayName, result.DisplayName);
        Assert.Equal(statusSeverity, result.StatusSeverity);
        Assert.Equal(statusDescription, result.StatusDescription);
    }

    [Fact]
    public async Task GetRoadStatusAsync_MixedCaseJson_ReturnsRoadStatus()
    {
        const string displayName = "A2";
        const string statusSeverity = "Good";
        const string statusDescription = "No Exceptional Delays";

        var jsonResponse = "[{\"DisplayName\":\"" + displayName + "\",\"statusSeverity\":\"" + statusSeverity + "\",\"STATUSSEVERITYDESCRIPTION\":\"" + statusDescription + "\"}]";

        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, jsonResponse);
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");
        var result = await client.GetRoadStatusAsync(roadId);

        Assert.Equal(displayName, result.DisplayName);
        Assert.Equal(statusSeverity, result.StatusSeverity);
        Assert.Equal(statusDescription, result.StatusDescription);
    }

    [Fact]
    public async Task GetRoadStatusAsync_EmptyDisplayName_ThrowsUnknownRoadException()
    {
        var roadStatusDto = new
        {
            displayName = "",
            statusSeverity = "Good",
            statusSeverityDescription = "No Exceptional Delays"
        };
        var jsonResponse = JsonSerializer.Serialize(new[] { roadStatusDto });

        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, jsonResponse);
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<UnknownRoadException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Equal("A2 is not a valid road", exception.Message);
    }

    [Fact]
    public async Task GetRoadStatusAsync_WhitespaceStatusSeverity_ThrowsUnknownRoadException()
    {
        var roadStatusDto = new
        {
            displayName = "A2",
            statusSeverity = "   ",
            statusSeverityDescription = "No Exceptional Delays"
        };
        var jsonResponse = JsonSerializer.Serialize(new[] { roadStatusDto });

        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, jsonResponse);
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<UnknownRoadException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Equal("A2 is not a valid road", exception.Message);
    }

    [Fact]
    public async Task GetRoadStatusAsync_WithAppIdAndAppKey_IncludesCredentialsInUrl()
    {
        const string displayName = "A2";
        const string statusSeverity = "Good";
        const string statusDescription = "No Exceptional Delays";
        const string appId = "test-app-id";
        const string appKey = "test-app-key";

        var roadStatusDto = new
        {
            displayName,
            statusSeverity,
            statusSeverityDescription = statusDescription
        };
        var jsonResponse = JsonSerializer.Serialize(new[] { roadStatusDto });

        string? capturedUrl = null;
        var handler = new TestHttpMessageHandlerWithUrlCapture(HttpStatusCode.OK, jsonResponse, url => capturedUrl = url);
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient, appId: appId, appKey: appKey);

        var roadId = RoadId.Parse("A2");
        var result = await client.GetRoadStatusAsync(roadId);

        Assert.NotNull(capturedUrl);
        Assert.Contains($"app_id={Uri.EscapeDataString(appId)}", capturedUrl);
        Assert.Contains($"app_key={Uri.EscapeDataString(appKey)}", capturedUrl);
        Assert.Equal(displayName, result.DisplayName);
        Assert.Equal(statusSeverity, result.StatusSeverity);
        Assert.Equal(statusDescription, result.StatusDescription);
    }

    [Fact]
    public async Task GetRoadStatusAsync_NullRoadStatuses_ThrowsUnknownRoadException()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "null");
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<UnknownRoadException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Equal("A2 is not a valid road", exception.Message);
    }

    [Fact]
    public async Task GetRoadStatusAsync_EmptyRoadStatusesArray_ThrowsUnknownRoadException()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "[]");
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<UnknownRoadException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Equal("A2 is not a valid road", exception.Message);
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

    private sealed class TestHttpMessageHandlerWithUrlCapture : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;
        private readonly Action<string> _urlCapture;

        public TestHttpMessageHandlerWithUrlCapture(HttpStatusCode statusCode, string content, Action<string> urlCapture)
        {
            _statusCode = statusCode;
            _content = content;
            _urlCapture = urlCapture;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _urlCapture(request.RequestUri?.ToString() ?? string.Empty);
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}
