using RoadStatus.Core;

namespace RoadStatus.Cli;

internal static class Program
{
    internal const int ExitCodeSuccess = 0;
    internal const int ExitCodeInvalidRoad = 1;
    internal const int ExitCodeInvalidUsage = 2;

    private static async Task<int> Main(string[] args)
    {
        var parser = new CliArgumentParser();
        var httpClient = new HttpClient();
        var client = new TflRoadStatusClient(httpClient);
        var app = new CliApplication(parser, client);

        return await app.RunAsync(args, Console.Out);
    }
}
