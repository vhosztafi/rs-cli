namespace RoadStatus.Core;

public class RoadStatusException : Exception
{
    public RoadStatusException(string message)
        : base(message)
    {
    }

    public RoadStatusException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}