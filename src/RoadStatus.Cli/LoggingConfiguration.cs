using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace RoadStatus.Cli;

internal static class LoggingConfiguration
{
    internal static ILoggerFactory ConfigureLogging(bool verbose, bool quiet)
    {
        Serilog.Events.LogEventLevel level;
        if (verbose)
        {
            level = Serilog.Events.LogEventLevel.Debug;
        }
        else if (quiet)
        {
            level = Serilog.Events.LogEventLevel.Error;
        }
        else
        {
            level = Serilog.Events.LogEventLevel.Error;
        }
        
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(level)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "RoadStatus.Cli")
            .Enrich.WithProperty("Version", version)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        
        return new SerilogLoggerFactory(Log.Logger);
    }
    
    internal static bool IsVerboseEnabled(bool cliVerbose)
    {
        if (cliVerbose)
        {
            return true;
        }
        
        var roadStatusVerbose = Environment.GetEnvironmentVariable("ROADSTATUS_VERBOSE");
        var tflVerbose = Environment.GetEnvironmentVariable("TFL_VERBOSE");
        
        var envValue = roadStatusVerbose ?? tflVerbose;
        if (string.IsNullOrWhiteSpace(envValue))
        {
            return false;
        }
        
        return envValue.Equals("1", StringComparison.OrdinalIgnoreCase) ||
               envValue.Equals("true", StringComparison.OrdinalIgnoreCase);
    }
    
    internal static bool IsQuietEnabled(bool cliQuiet)
    {
        if (cliQuiet)
        {
            return true;
        }
        
        var roadStatusQuiet = Environment.GetEnvironmentVariable("ROADSTATUS_QUIET");
        var tflQuiet = Environment.GetEnvironmentVariable("TFL_QUIET");
        
        var envValue = roadStatusQuiet ?? tflQuiet;
        if (string.IsNullOrWhiteSpace(envValue))
        {
            return false;
        }
        
        return envValue.Equals("1", StringComparison.OrdinalIgnoreCase) ||
               envValue.Equals("true", StringComparison.OrdinalIgnoreCase);
    }
}