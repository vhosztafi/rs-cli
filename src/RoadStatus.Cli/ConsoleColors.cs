namespace RoadStatus.Cli;

internal static class ConsoleColors
{
    private static readonly bool ColorsEnabled = ShouldEnableColors();

    private static bool ShouldEnableColors()
    {
        if (Console.IsOutputRedirected)
        {
            return false;
        }

        var noColor = Environment.GetEnvironmentVariable("NO_COLOR");
        if (!string.IsNullOrWhiteSpace(noColor))
        {
            return false;
        }

        return true;
    }

    public static string Cyan(string text) => ColorsEnabled ? $"\u001b[36m{text}\u001b[0m" : text;

    public static string Yellow(string text) => ColorsEnabled ? $"\u001b[33m{text}\u001b[0m" : text;

    public static string Green(string text) => ColorsEnabled ? $"\u001b[32m{text}\u001b[0m" : text;

    public static string Bold(string text) => ColorsEnabled ? $"\u001b[1m{text}\u001b[0m" : text;

    public static string Reset => ColorsEnabled ? "\u001b[0m" : string.Empty;
}