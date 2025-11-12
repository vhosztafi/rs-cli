using System.Net;
using System.Net.Http.Json;

namespace RoadStatus.Core;

public class TflRoadStatusClient : ITflRoadStatusClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public TflRoadStatusClient(HttpClient httpClient, string baseUrl = "https://api.tfl.gov.uk")
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<RoadStatus> GetRoadStatusAsync(RoadId roadId)
    {
        var url = $"{_baseUrl}/Road/{roadId}";
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
