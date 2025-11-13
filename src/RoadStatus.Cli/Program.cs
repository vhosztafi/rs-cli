using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.Logging;
using RoadStatus.Core;
using Serilog;

namespace RoadStatus.Cli;

internal static class Program
{
    internal const int ExitCodeSuccess = 0;
    internal const int ExitCodeInvalidRoad = 1;
    internal const int ExitCodeInvalidUsage = 2;

    private static async Task<int> Main(string[] args)
    {
        var isHelpRequested = args.Length > 0 && (args[0] == "--help" || args[0] == "-h" || args[0] == "-?" || args[0] == "/?");
        var isVersionRequested = args.Length > 0 && (args[0] == "--version" || args[0] == "-v");

        if (isHelpRequested || isVersionRequested)
        {
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(new ColoredTextWriter(originalOut));
            }
            catch
            {
                // ignored
            }
        }

        var roadIdsArgument = new Argument<string[]>("road-ids")
        {
            Arity = ArgumentArity.OneOrMore,
            Description = "One or more road IDs to check (e.g., A2, A3)"
        };

        var jsonOption = new Option<bool>("--json", "Output results in JSON format");
        jsonOption.AddAlias("-j");

        var verboseOption = new Option<bool>("--verbose", "Enable verbose (Debug) logging");
        verboseOption.AddAlias("-V");

        var quietOption = new Option<bool>("--quiet", "Enable quiet mode (Error-level logging only, default behavior)");
        quietOption.AddAlias("-q");

        var rootCommand = new RootCommand("Query the TfL Road API to display road status information.")
        {
            roadIdsArgument,
            jsonOption,
            verboseOption,
            quietOption
        };

        rootCommand.SetHandler(async (InvocationContext context) =>
        {
            var roadIds = context.ParseResult.GetValueForArgument(roadIdsArgument);
            var json = context.ParseResult.GetValueForOption(jsonOption);
            var cliVerbose = context.ParseResult.GetValueForOption(verboseOption);
            var cliQuiet = context.ParseResult.GetValueForOption(quietOption);

            var verbose = LoggingConfiguration.IsVerboseEnabled(cliVerbose);
            var quiet = LoggingConfiguration.IsQuietEnabled(cliQuiet);
            
            if (verbose)
            {
                quiet = false;
            }

            var loggerFactory = LoggingConfiguration.ConfigureLogging(verbose, quiet);

            var httpClientFactory = new HttpClientFactory();
            var httpClient = httpClientFactory.Create();
            var appId = Environment.GetEnvironmentVariable("TFL_APP_ID");
            var appKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
            
            var clientLogger = loggerFactory.CreateLogger<TflRoadStatusClient>();
            var client = new TflRoadStatusClient(httpClient, appId: appId, appKey: appKey, logger: clientLogger);
            
            var formatter = new RoadStatusFormatter();
            var appLogger = loggerFactory.CreateLogger<CliApplication>();
            var app = new CliApplication(client, formatter, appLogger);

            var exitCode = await app.RunAsync(roadIds, json, Console.Out);
            context.ExitCode = exitCode;
        });

        try
        {
            return await rootCommand.InvokeAsync(args);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
