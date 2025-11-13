namespace RoadStatus.Cli;

public sealed class RoadStatusFormatter
{
    public string Format(Core.RoadStatus roadStatus)
    {
        return $"The status of the {roadStatus.DisplayName} is as follows\r\n        Road Status is {roadStatus.StatusSeverity}\r\n        Road Status Description is {roadStatus.StatusDescription}\r\n";
    }
}

