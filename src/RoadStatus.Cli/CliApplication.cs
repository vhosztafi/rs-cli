using RoadStatus.Core;

namespace RoadStatus.Cli;

public class CliApplication
{
    private readonly ITflRoadStatusClient _client;
    private readonly RoadStatusFormatter _formatter;

    public CliApplication(ITflRoadStatusClient client, RoadStatusFormatter formatter)
    {
        _client = client;
        _formatter = formatter;
    }

    public async Task<int> RunAsync(string[] roadIds, bool jsonOutput, TextWriter output)
    {
        if (roadIds.Length == 0)
        {
            await output.WriteLineAsync("At least one road ID is required.");
            return Program.ExitCodeInvalidUsage;
        }

        var roadStatuses = new List<Core.RoadStatus>();
        var errors = new List<string>();
        var hasInvalidRoad = false;

        foreach (var roadIdString in roadIds)
        {
            try
            {
                var roadId = RoadId.Parse(roadIdString);
                var roadStatus = await _client.GetRoadStatusAsync(roadId);
                roadStatuses.Add(roadStatus);
            }
            catch (UnknownRoadException ex)
            {
                errors.Add(ex.Message);
                hasInvalidRoad = true;
            }
        }

        if (jsonOutput)
        {
            var jsonOutputText = _formatter.FormatJson(roadStatuses);
            await output.WriteLineAsync(jsonOutputText);
        }
        else
        {
            foreach (var roadStatus in roadStatuses)
            {
                var formattedOutput = _formatter.Format(roadStatus);
                await output.WriteAsync(formattedOutput);
            }
        }

        foreach (var error in errors)
        {
            await output.WriteLineAsync(error);
        }

        return hasInvalidRoad ? Program.ExitCodeInvalidRoad : Program.ExitCodeSuccess;
    }
}
