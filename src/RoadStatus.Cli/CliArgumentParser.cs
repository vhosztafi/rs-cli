namespace RoadStatus.Cli;

public class CliArgumentParser
{
    public virtual CliArgumentParseResult Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return CliArgumentParseResult.InvalidUsage("No arguments provided.");
        }

        if (args.Length > 1)
        {
            return CliArgumentParseResult.InvalidUsage("Too many arguments provided.");
        }

        var arg = args[0];
        
        if (arg == "--help" || arg == "-h" || arg == "/?")
        {
            return CliArgumentParseResult.ShowHelp();
        }

        if (arg == "--version" || arg == "-v")
        {
            return CliArgumentParseResult.ShowVersion();
        }

        return CliArgumentParseResult.Success(arg);
    }
}

public sealed class CliArgumentParseResult
{
    public bool IsSuccess { get; }
    public bool ShouldShowHelp { get; }
    public bool ShouldShowVersion { get; }
    public string? RoadId { get; }
    public string? ErrorMessage { get; }

    private CliArgumentParseResult(bool isSuccess, bool shouldShowHelp, bool shouldShowVersion, string? roadId, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ShouldShowHelp = shouldShowHelp;
        ShouldShowVersion = shouldShowVersion;
        RoadId = roadId;
        ErrorMessage = errorMessage;
    }

    public static CliArgumentParseResult Success(string roadId) =>
        new(true, false, false, roadId, null);

    public static CliArgumentParseResult InvalidUsage(string errorMessage) =>
        new(false, false, false, null, errorMessage);

    public static CliArgumentParseResult ShowHelp() =>
        new(false, true, false, null, null);

    public static CliArgumentParseResult ShowVersion() =>
        new(false, false, true, null, null);
}

