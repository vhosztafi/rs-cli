using RoadStatus.Core;

namespace RoadStatus.Cli;

public sealed class CliApplication
{
    private readonly CliArgumentParser _parser;
    private readonly ITflRoadStatusClient _client;
    private readonly RoadStatusFormatter _formatter;

    public CliApplication(CliArgumentParser parser, ITflRoadStatusClient client, RoadStatusFormatter formatter)
    {
        _parser = parser;
        _client = client;
        _formatter = formatter;
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

            var formattedOutput = _formatter.Format(roadStatus);
            await output.WriteAsync(formattedOutput);

            return Program.ExitCodeSuccess;
        }
        catch (UnknownRoadException ex)
        {
            await output.WriteLineAsync(ex.Message);
            return Program.ExitCodeInvalidRoad;
        }
    }
}
