using RoadStatus.Core;

namespace RoadStatus.Cli;

public sealed class CliApplication
{
    private readonly CliArgumentParser _parser;
    private readonly ITflRoadStatusClient _client;

    public CliApplication(CliArgumentParser parser, ITflRoadStatusClient client)
    {
        _parser = parser;
        _client = client;
    }

    public async Task<int> RunAsync(string[] args, TextWriter output)
    {
        var parseResult = _parser.Parse(args);

        if (!parseResult.IsSuccess)
        {
            await output.WriteLineAsync(parseResult.ErrorMessage);
            return Program.ExitCodeInvalidUsage;
        }

        try
        {
            var roadId = RoadId.Parse(parseResult.RoadId!);
            var roadStatus = await _client.GetRoadStatusAsync(roadId);

            await output.WriteLineAsync($"Road Status: {roadStatus.DisplayName}");
            await output.WriteLineAsync($"Status: {roadStatus.StatusSeverity}");
            await output.WriteLineAsync($"Description: {roadStatus.StatusDescription}");

            return Program.ExitCodeSuccess;
        }
        catch (UnknownRoadException ex)
        {
            await output.WriteLineAsync(ex.Message);
            return Program.ExitCodeInvalidRoad;
        }
    }
}
