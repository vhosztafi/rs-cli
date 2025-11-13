using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace RoadStatus.Core;

public class TflRoadStatusClient : ITflRoadStatusClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string? _appId;
    private readonly string? _appKey;
    private readonly ILogger<TflRoadStatusClient> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TflRoadStatusClient(HttpClient httpClient, string baseUrl = "https://api.tfl.gov.uk", string? appId = null, string? appKey = null, ILogger<TflRoadStatusClient>? logger = null)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _appId = appId;
        _appKey = appKey;
        _logger = logger ?? NullLogger<TflRoadStatusClient>.Instance;
        _retryPolicy = CreateRetryPolicy();
    }

    private AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var requestId = context.ContainsKey("RequestId") ? context["RequestId"]?.ToString() : "unknown";
                    var roadId = context.ContainsKey("RoadId") ? context["RoadId"]?.ToString() : "unknown";

                    if (outcome.Exception != null)
                    {
                        _logger.LogDebug(
                            "Retrying HTTP request due to exception {RequestId} {RoadId} {RetryCount} {ExceptionType} {DelayMs}ms",
                            requestId,
                            roadId,
                            retryCount,
                            outcome.Exception.GetType().Name,
                            timespan.TotalMilliseconds);
                    }
                    else if (outcome.Result != null)
                    {
                        _logger.LogDebug(
                            "Retrying HTTP request due to status code {RequestId} {RoadId} {RetryCount} {StatusCode} {DelayMs}ms",
                            requestId,
                            roadId,
                            retryCount,
                            (int)outcome.Result.StatusCode,
                            timespan.TotalMilliseconds);
                    }
                });
    }

    public async Task<RoadStatus> GetRoadStatusAsync(RoadId roadId)
    {
        var startTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString("N")[..8]; // Short request ID
        var roadIdValue = roadId.ToString();

        var url = $"{_baseUrl}/Road/{roadId}";
        if (!string.IsNullOrWhiteSpace(_appId) && !string.IsNullOrWhiteSpace(_appKey))
        {
            url += $"?app_id={Uri.EscapeDataString(_appId)}&app_key={Uri.EscapeDataString(_appKey)}";
        }

        var hasCredentials = !string.IsNullOrWhiteSpace(_appId) && !string.IsNullOrWhiteSpace(_appKey);

        _logger.LogDebug(
            "Initiating HTTP request to TfL API {RequestId} {RoadId} {RequestUrl} {HasCredentials}",
            requestId,
            roadIdValue,
            url,
            hasCredentials);

        HttpResponseMessage response;
        try
        {
            var context = new Context
            {
                ["RequestId"] = requestId,
                ["RoadId"] = roadIdValue
            };

            response = await _retryPolicy.ExecuteAsync(async _ =>
            {
                return await _httpClient.GetAsync(url);
            }, context);
        }
        catch (HttpRequestException ex)
        {
            var totalTime = DateTime.UtcNow - startTime;
            _logger.LogError(
                ex,
                "Network error while connecting to TfL API {RequestId} {RoadId} {ExecutionTimeMs}ms",
                requestId,
                roadIdValue,
                totalTime.TotalMilliseconds);
            throw new RoadStatusException(
                $"Unable to connect to TfL API. Please check your internet connection and try again.",
                ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            var totalTime = DateTime.UtcNow - startTime;
            _logger.LogError(
                ex,
                "Request timeout while connecting to TfL API {RequestId} {RoadId} {ExecutionTimeMs}ms",
                requestId,
                roadIdValue,
                totalTime.TotalMilliseconds);
            throw new RoadStatusException(
                $"Request to TfL API timed out. The service may be temporarily unavailable. Please try again later.",
                ex);
        }
        catch (TaskCanceledException ex)
        {
            var totalTime = DateTime.UtcNow - startTime;
            _logger.LogError(
                ex,
                "Request was cancelled while connecting to TfL API {RequestId} {RoadId} {ExecutionTimeMs}ms",
                requestId,
                roadIdValue,
                totalTime.TotalMilliseconds);
            throw new RoadStatusException(
                $"Request to TfL API was cancelled. Please try again.",
                ex);
        }

        var responseTime = DateTime.UtcNow - startTime;
        var statusCode = (int)response.StatusCode;

        _logger.LogDebug(
            "HTTP response received {RequestId} {RoadId} {StatusCode} {ResponseTimeMs}ms",
            requestId,
            roadIdValue,
            statusCode,
            responseTime.TotalMilliseconds);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning(
                "Road not found in TfL API {RequestId} {RoadId} {StatusCode} {ResponseTimeMs}ms",
                requestId,
                roadIdValue,
                statusCode,
                responseTime.TotalMilliseconds);
            throw new UnknownRoadException(roadIdValue);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var totalTime = DateTime.UtcNow - startTime;
            _logger.LogError(
                "HTTP error response from TfL API {RequestId} {RoadId} {StatusCode} {ResponseTimeMs}ms {ErrorContent}",
                requestId,
                roadIdValue,
                statusCode,
                totalTime.TotalMilliseconds,
                errorContent);
            throw new RoadStatusException(
                $"TfL API returned an error (HTTP {statusCode}). Please try again later.");
        }

        RoadStatusDto[]? roadStatuses;
        try
        {
            roadStatuses = await response.Content.ReadFromJsonAsync<RoadStatusDto[]>(JsonOptions);
        }
        catch (JsonException ex)
        {
            var totalTime = DateTime.UtcNow - startTime;
            _logger.LogError(
                ex,
                "JSON parse error from TfL API response {RequestId} {RoadId} {StatusCode} {ResponseTimeMs}ms",
                requestId,
                roadIdValue,
                statusCode,
                totalTime.TotalMilliseconds);
            throw new RoadStatusException(
                $"Invalid response format from TfL API. The service may be experiencing issues. Please try again later.",
                ex);
        }

        var totalTimeAfterParse = DateTime.UtcNow - startTime;

        if (roadStatuses == null || roadStatuses.Length == 0)
        {
            _logger.LogError(
                "Empty or null response from TfL API {RequestId} {RoadId} {StatusCode} {ResponseTimeMs}ms",
                requestId,
                roadIdValue,
                statusCode,
                totalTimeAfterParse.TotalMilliseconds);
            throw new UnknownRoadException(roadIdValue);
        }

        var roadStatus = roadStatuses[0];
        if (string.IsNullOrWhiteSpace(roadStatus.DisplayName) ||
            string.IsNullOrWhiteSpace(roadStatus.StatusSeverity) ||
            string.IsNullOrWhiteSpace(roadStatus.StatusSeverityDescription))
        {
            _logger.LogError(
                "Invalid response data from TfL API {RequestId} {RoadId} {StatusCode} {ResponseTimeMs}ms",
                requestId,
                roadIdValue,
                statusCode,
                totalTimeAfterParse.TotalMilliseconds);
            throw new UnknownRoadException(roadIdValue);
        }

        _logger.LogInformation(
            "Successfully retrieved road status from TfL API {RequestId} {RoadId} {DisplayName} {StatusSeverity} {StatusCode} {ResponseTimeMs}ms",
            requestId,
            roadIdValue,
            roadStatus.DisplayName,
            roadStatus.StatusSeverity,
            statusCode,
            totalTimeAfterParse.TotalMilliseconds);

        return new RoadStatus(
            roadStatus.DisplayName,
            roadStatus.StatusSeverity,
            roadStatus.StatusSeverityDescription);
    }
}
