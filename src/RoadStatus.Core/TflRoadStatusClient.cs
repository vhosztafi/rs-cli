using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RoadStatus.Core;

public class TflRoadStatusClient : ITflRoadStatusClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string? _appId;
    private readonly string? _appKey;
    private readonly ILogger<TflRoadStatusClient> _logger;
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
        
        var response = await _httpClient.GetAsync(url);
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
            _logger.LogError(
                "HTTP error response from TfL API {RequestId} {RoadId} {StatusCode} {ResponseTimeMs}ms {ErrorContent}",
                requestId,
                roadIdValue,
                statusCode,
                responseTime.TotalMilliseconds,
                errorContent);
        }

        response.EnsureSuccessStatusCode();

        var roadStatuses = await response.Content.ReadFromJsonAsync<RoadStatusDto[]>(JsonOptions);
        var totalTime = DateTime.UtcNow - startTime;

        if (roadStatuses == null || roadStatuses.Length == 0)
        {
            _logger.LogError(
                "Empty or null response from TfL API {RequestId} {RoadId} {StatusCode} {ResponseTimeMs}ms",
                requestId,
                roadIdValue,
                statusCode,
                totalTime.TotalMilliseconds);
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
                totalTime.TotalMilliseconds);
            throw new UnknownRoadException(roadIdValue);
        }

        _logger.LogInformation(
            "Successfully retrieved road status from TfL API {RequestId} {RoadId} {DisplayName} {StatusSeverity} {StatusCode} {ResponseTimeMs}ms",
            requestId,
            roadIdValue,
            roadStatus.DisplayName,
            roadStatus.StatusSeverity,
            statusCode,
            totalTime.TotalMilliseconds);

        return new RoadStatus(
            roadStatus.DisplayName,
            roadStatus.StatusSeverity,
            roadStatus.StatusSeverityDescription);
    }
}
