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

    [Fact]
    public async Task GetRoadStatusAsync_RetriesOnTransientHttpError_EventuallySucceeds()
    {
        var callCount = 0;
        var handler = new TestHttpMessageHandlerWithRetry(
            HttpStatusCode.InternalServerError,
            HttpStatusCode.OK,
            JsonSerializer.Serialize(new[]
            {
                new
                {
                    displayName = "A2",
                    statusSeverity = "Good",
                    statusSeverityDescription = "No Exceptional Delays"
                }
            }),
            () => callCount++);

        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");
        var result = await client.GetRoadStatusAsync(roadId);

        Assert.Equal("A2", result.DisplayName);
        Assert.True(callCount >= 2, "Expected at least 2 retry attempts");
    }

    [Fact]
    public async Task GetRoadStatusAsync_RetriesOnTooManyRequests_EventuallySucceeds()
    {
        var callCount = 0;
        var handler = new TestHttpMessageHandlerWithRetry(
            HttpStatusCode.TooManyRequests,
            HttpStatusCode.OK,
            JsonSerializer.Serialize(new[]
            {
                new
                {
                    displayName = "A2",
                    statusSeverity = "Good",
                    statusSeverityDescription = "No Exceptional Delays"
                }
            }),
            () => callCount++);

        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");
        var result = await client.GetRoadStatusAsync(roadId);

        Assert.Equal("A2", result.DisplayName);
        Assert.True(callCount >= 2, "Expected at least 2 retry attempts");
    }

    [Fact]
    public async Task GetRoadStatusAsync_NetworkError_ThrowsRoadStatusException()
    {
        var handler = new TestHttpMessageHandlerWithException(new HttpRequestException("Network error"));
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<RoadStatusException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Contains("Unable to connect to TfL API", exception.Message);
        Assert.Contains("check your internet connection", exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.IsType<HttpRequestException>(exception.InnerException);
    }

    [Fact]
    public async Task GetRoadStatusAsync_TimeoutError_ThrowsRoadStatusException()
    {
        var handler = new TestHttpMessageHandlerWithException(
            new TaskCanceledException("Request timed out", new TimeoutException()));
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<RoadStatusException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Contains("timed out", exception.Message);
        Assert.Contains("temporarily unavailable", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task GetRoadStatusAsync_CancelledRequest_ThrowsRoadStatusException()
    {
        var handler = new TestHttpMessageHandlerWithException(
            new TaskCanceledException("Request was cancelled"));
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<RoadStatusException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Contains("Request to TfL API was cancelled", exception.Message);
        Assert.Contains("Please try again", exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.IsType<TaskCanceledException>(exception.InnerException);
    }

    [Fact]
    public async Task GetRoadStatusAsync_JsonParseError_ThrowsRoadStatusException()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "invalid json {");
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<RoadStatusException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Contains("Invalid response format", exception.Message);
        Assert.Contains("experiencing issues", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task GetRoadStatusAsync_NonSuccessStatusCode_ThrowsRoadStatusException()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.BadRequest, "Error message");
        var httpClient = new HttpClient(handler);
        var client = new TflRoadStatusClient(httpClient);

        var roadId = RoadId.Parse("A2");

        var exception = await Assert.ThrowsAsync<RoadStatusException>(
            () => client.GetRoadStatusAsync(roadId));

        Assert.Contains("TfL API returned an error", exception.Message);
        Assert.Contains("HTTP 400", exception.Message);
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

    private sealed class TestHttpMessageHandlerWithRetry : HttpMessageHandler
    {
        private readonly HttpStatusCode _initialStatusCode;
        private readonly HttpStatusCode _finalStatusCode;
        private readonly string _content;
        private readonly Action _onCall;
        private int _callCount;

        public TestHttpMessageHandlerWithRetry(
            HttpStatusCode initialStatusCode,
            HttpStatusCode finalStatusCode,
            string content,
            Action onCall)
        {
            _initialStatusCode = initialStatusCode;
            _finalStatusCode = finalStatusCode;
            _content = content;
            _onCall = onCall;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _onCall();
            _callCount++;

            var statusCode = _callCount <= 2 ? _initialStatusCode : _finalStatusCode;
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }

    private sealed class TestHttpMessageHandlerWithException : HttpMessageHandler
    {
        private readonly Exception _exception;

        public TestHttpMessageHandlerWithException(Exception exception)
        {
            _exception = exception;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromException<HttpResponseMessage>(_exception);
        }
    }
}
