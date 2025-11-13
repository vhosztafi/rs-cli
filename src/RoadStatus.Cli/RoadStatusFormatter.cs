using System.Text.Json;

namespace RoadStatus.Cli;

public sealed class RoadStatusFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    public string Format(Core.RoadStatus roadStatus)
    {
        return $"The status of the {roadStatus.DisplayName} is as follows\r\n        Road Status is {roadStatus.StatusSeverity}\r\n        Road Status Description is {roadStatus.StatusDescription}\r\n";
    }

    public string FormatJson(IEnumerable<Core.RoadStatus> roadStatuses)
    {
        var roadStatusArray = roadStatuses.Select(rs => new
        {
            displayName = rs.DisplayName,
            statusSeverity = rs.StatusSeverity,
            statusDescription = rs.StatusDescription
        }).ToArray();

        return JsonSerializer.Serialize(roadStatusArray, JsonOptions);
    }
}

