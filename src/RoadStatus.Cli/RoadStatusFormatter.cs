using RoadStatus.Core;

namespace RoadStatus.Cli;

public sealed class RoadStatusFormatter
{
    public string Format(Core.RoadStatus roadStatus)
    {
        return $"The status of the {roadStatus.DisplayName} is {roadStatus.StatusSeverity}\r\n{roadStatus.StatusDescription}\r\n";
    }
}

