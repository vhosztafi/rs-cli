namespace RoadStatus.Core;

internal sealed record RoadStatusDto(
    string DisplayName,
    string StatusSeverity,
    string StatusSeverityDescription);
