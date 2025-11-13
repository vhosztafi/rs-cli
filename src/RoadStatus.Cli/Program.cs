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
        var httpClientFactory = new HttpClientFactory();
        var httpClient = httpClientFactory.Create();
        var appId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var appKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        var client = new TflRoadStatusClient(httpClient, appId: appId, appKey: appKey);
        var formatter = new RoadStatusFormatter();
        var app = new CliApplication(parser, client, formatter);

        return await app.RunAsync(args, Console.Out);
    }
}
