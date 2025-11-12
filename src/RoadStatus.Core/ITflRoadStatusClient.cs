namespace RoadStatus.Core;

public interface ITflRoadStatusClient
{
    Task<RoadStatus> GetRoadStatusAsync(RoadId roadId);
}
