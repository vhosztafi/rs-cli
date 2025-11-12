namespace RoadStatus.Core;

public class UnknownRoadException : Exception
{
    public UnknownRoadException(string id)
        : base($"{id} is not a valid road")
    {
    }
}
