namespace RoadStatus.Cli;

internal static class ConsoleColors
{
    private static readonly bool ColorsEnabled = ShouldEnableColors();

    private static bool ShouldEnableColors()
    {
        var noColor = Environment.GetEnvironmentVariable("NO_COLOR");
        if (!string.IsNullOrWhiteSpace(noColor))
        {
            return false;
        }

        return true;
    }

    public static string? Cyan(string? text)
    {
        if (text == null) return null;
        if (text.Length == 0) return text;
        return ApplyColor(text, "\u001b[36m");
    }

    public static string? Yellow(string? text)
    {
        if (text == null) return null;
        if (text.Length == 0) return text;
        return ApplyColor(text, "\u001b[33m");
    }

    public static string? Green(string? text)
    {
        if (text == null) return null;
        if (text.Length == 0) return text;
        return ApplyColor(text, "\u001b[32m");
    }

    public static string? Bold(string? text)
    {
        if (text == null) return null;
        if (text.Length == 0) return text;
        return ApplyColor(text, "\u001b[1m");
    }

    public static string Reset => ColorsEnabled ? "\u001b[0m" : string.Empty;

    private static string ApplyColor(string text, string ansiCode)
    {
        if (ColorsEnabled)
        {
            return $"{ansiCode}{text}\u001b[0m";
        }
        return text;
    }
}