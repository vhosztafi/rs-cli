using System.Text;

namespace RoadStatus.Cli;

internal class ColoredTextWriter : TextWriter
{
    private readonly TextWriter _innerWriter;
    private readonly StringBuilder _lineBuffer = new();
    private readonly bool _enableColors;

    internal static IReadOnlyList<string> KnownOptions { get; private set; } =
    [
        "-j", "--json", "--version", "-?", "-h", "--help", "-V", "--verbose", "-q", "--quiet"
    ];

    internal static void SetKnownOptions(IEnumerable<string>? options)
    {
        if (options == null)
        {
            return;
        }

        var list = options.Where(o => !string.IsNullOrWhiteSpace(o))
            .Select(o => o.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (list.Length > 0)
        {
            KnownOptions = list;
        }
    }

    public ColoredTextWriter(TextWriter innerWriter, bool enableColors = true)
    {
        _innerWriter = innerWriter;
        _enableColors = enableColors && !Console.IsOutputRedirected &&
                       string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("NO_COLOR"));
    }

    public override Encoding Encoding => _innerWriter.Encoding;

    public override void Write(char value)
    {
        if (value == '\n')
        {
            FlushLine();
        }
        else
        {
            _lineBuffer.Append(value);
        }
    }

    public override void Write(string? value)
    {
        if (value == null)
        {
            return;
        }

        var lines = value.Split('\n');
        for (int i = 0; i < lines.Length - 1; i++)
        {
            _lineBuffer.Append(lines[i]);
            FlushLine();
        }
        if (lines.Length > 0)
        {
            _lineBuffer.Append(lines[^1]);
        }
    }

    public override void Flush()
    {
        if (_lineBuffer.Length > 0)
        {
            FlushLine();
        }
        _innerWriter.Flush();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Flush();
        }
        base.Dispose(disposing);
    }

    private void FlushLine()
    {
        if (_lineBuffer.Length == 0)
        {
            _innerWriter.Write('\n');
            return;
        }

        var line = _lineBuffer.ToString();
        _lineBuffer.Clear();

        var coloredLine = _enableColors ? ApplyColorsToLine(line) : line;
        _innerWriter.Write(coloredLine);
        _innerWriter.Write('\n');
    }

    private static string ApplyColorsToLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return line;
        }

        var trimmed = line.TrimStart();
        var leadingSpaces = line.Substring(0, line.Length - trimmed.Length);

        var coloredLine = new StringBuilder();
        coloredLine.Append(leadingSpaces);

        var remaining = trimmed;
        var lastIndex = 0;
        var foundOption = false;

        while (lastIndex < remaining.Length)
        {
            var bestMatch = (option: "", index: -1, length: 0);

            foreach (var option in KnownOptions)
            {
                var index = remaining.IndexOf(option, lastIndex, StringComparison.Ordinal);
                if (index >= 0 && (index == 0 || !char.IsLetterOrDigit(remaining[index - 1])) &&
                    (index + option.Length >= remaining.Length || !char.IsLetterOrDigit(remaining[index + option.Length])))
                {
                    if (bestMatch.index < 0 || index < bestMatch.index)
                    {
                        bestMatch = (option, index, option.Length);
                    }
                }
            }

            if (bestMatch.index >= 0)
            {
                foundOption = true;
                coloredLine.Append(remaining.Substring(lastIndex, bestMatch.index - lastIndex));
                coloredLine.Append(ConsoleColors.Yellow(bestMatch.option));
                lastIndex = bestMatch.index + bestMatch.length;
            }
            else
            {
                coloredLine.Append(remaining.Substring(lastIndex));
                break;
            }
        }

        if (foundOption)
        {
            return coloredLine.ToString();
        }

        if (line.Contains('<') && line.Contains('>'))
        {
            var result = new StringBuilder();
            var startIndex = 0;
            while (startIndex < line.Length)
            {
                var angleStart = line.IndexOf('<', startIndex);
                if (angleStart < 0)
                {
                    result.Append(line.Substring(startIndex));
                    break;
                }
                result.Append(line.Substring(startIndex, angleStart - startIndex));
                var angleEnd = line.IndexOf('>', angleStart);
                if (angleEnd > angleStart)
                {
                    var argument = line.Substring(angleStart, angleEnd - angleStart + 1);
                    result.Append(ConsoleColors.Green(argument));
                    startIndex = angleEnd + 1;
                }
                else
                {
                    result.Append(line.Substring(angleStart));
                    break;
                }
            }
            return result.ToString();
        }
        else if (trimmed.EndsWith(':') && trimmed.Length < 50 && trimmed.Split(' ').Length <= 3)
        {
            return ConsoleColors.Bold(line) ?? line;
        }
        else if (trimmed.Length > 30 && !trimmed.StartsWith("  ") && !trimmed.StartsWith("-"))
        {
            return ConsoleColors.Cyan(line) ?? line;
        }

        return line;
    }
}
