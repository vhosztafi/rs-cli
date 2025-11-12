namespace RoadStatus.Cli;

public sealed class CliArgumentParser
{
    public CliArgumentParseResult Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return CliArgumentParseResult.InvalidUsage("No arguments provided.");
        }

        if (args.Length > 1)
        {
            return CliArgumentParseResult.InvalidUsage("Too many arguments provided.");
        }

        return CliArgumentParseResult.Success(args[0]);
    }
}

public sealed class CliArgumentParseResult
{
    public bool IsSuccess { get; }
    public string? RoadId { get; }
    public string? ErrorMessage { get; }

    private CliArgumentParseResult(bool isSuccess, string? roadId, string? errorMessage)
    {
        IsSuccess = isSuccess;
        RoadId = roadId;
        ErrorMessage = errorMessage;
    }

    public static CliArgumentParseResult Success(string roadId) =>
        new(true, roadId, null);

    public static CliArgumentParseResult InvalidUsage(string errorMessage) =>
        new(false, null, errorMessage);
}

