using System.Net;
using System.Net.Http.Json;

namespace RoadStatus.Core;

public class TflRoadStatusClient : ITflRoadStatusClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string? _appId;
    private readonly string? _appKey;

    public TflRoadStatusClient(HttpClient httpClient, string baseUrl = "https://api.tfl.gov.uk", string? appId = null, string? appKey = null)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _appId = appId;
        _appKey = appKey;
    }

    public async Task<RoadStatus> GetRoadStatusAsync(RoadId roadId)
    {
        var url = $"{_baseUrl}/Road/{roadId}";
        if (!string.IsNullOrWhiteSpace(_appId) && !string.IsNullOrWhiteSpace(_appKey))
        {
            url += $"?app_id={Uri.EscapeDataString(_appId)}&app_key={Uri.EscapeDataString(_appKey)}";
        }
        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new UnknownRoadException(roadId.ToString());
        }

        response.EnsureSuccessStatusCode();

        var roadStatuses = await response.Content.ReadFromJsonAsync<RoadStatusDto[]>();

        if (roadStatuses == null || roadStatuses.Length == 0)
        {
            throw new UnknownRoadException(roadId.ToString());
        }

        var roadStatus = roadStatuses[0];
        return new RoadStatus(
            roadStatus.DisplayName,
            roadStatus.StatusSeverity,
            roadStatus.StatusSeverityDescription);
    }
}
