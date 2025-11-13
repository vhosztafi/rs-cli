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

        if (parseResult.ShouldShowHelp)
        {
            await output.WriteLineAsync("Usage: RoadStatus <road-id>");
            await output.WriteLineAsync("");
            await output.WriteLineAsync("Arguments:");
            await output.WriteLineAsync("  <road-id>    The ID of the road to check (e.g., A2)");
            await output.WriteLineAsync("");
            await output.WriteLineAsync("Options:");
            await output.WriteLineAsync("  --help, -h, /?    Show this help message");
            await output.WriteLineAsync("  --version, -v     Show version information");
            await output.WriteLineAsync("");
            await output.WriteLineAsync("Environment Variables:");
            await output.WriteLineAsync("  TFL_APP_ID        TfL API application ID (optional)");
            await output.WriteLineAsync("  TFL_APP_KEY       TfL API application key (optional)");
            return Program.ExitCodeSuccess;
        }

        if (parseResult.ShouldShowVersion)
        {
            var version = typeof(Program).Assembly.GetName().Version;
            await output.WriteLineAsync($"RoadStatus CLI {version?.Major}.{version?.Minor}.{version?.Build ?? 0}");
            return Program.ExitCodeSuccess;
        }

        if (!parseResult.IsSuccess)
        {
            await output.WriteLineAsync(parseResult.ErrorMessage);
            return Program.ExitCodeInvalidUsage;
        }

        if (parseResult.RoadId == null)
        {
            await output.WriteLineAsync("Road ID is required.");
            return Program.ExitCodeInvalidUsage;
        }

        try
        {
            var roadId = RoadId.Parse(parseResult.RoadId);
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
