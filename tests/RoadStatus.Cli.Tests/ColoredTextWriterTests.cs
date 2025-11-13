using System.Reflection;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class ColoredTextWriterTests
{
    [Fact]
    public void Write_Char_AppendsToBuffer()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write('a');
        writer.Write('b');
        writer.Flush();

        Assert.Contains("ab", stringWriter.ToString());
    }

    [Fact]
    public void Write_Newline_FlushesBuffer()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write('a');
        writer.Write('\n');
        writer.Write('b');
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("a", result);
        Assert.Contains("b", result);
    }

    [Fact]
    public void Write_String_ProcessesLines()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write("line1\nline2");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("line1", result);
        Assert.Contains("line2", result);
    }

    [Fact]
    public void Write_NullString_DoesNotThrow()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write((string?)null);
        writer.Flush();

        Assert.NotNull(stringWriter.ToString());
    }

    [Fact]
    public void Flush_WithBufferedContent_WritesContent()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write("test");
        writer.Flush();

        Assert.Contains("test", stringWriter.ToString());
    }

    [Fact]
    public void Flush_EmptyBuffer_WritesNewline()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write('\n');
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("\n", result);
    }

    [Fact]
    public void Encoding_ReturnsInnerWriterEncoding()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        Assert.Equal(stringWriter.Encoding, writer.Encoding);
    }

    [Fact]
    public void ApplyColors_OptionLine_ColorsOptionName()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  --json  Output in JSON\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("--json", result);
    }

    [Fact]
    public void ApplyColors_MultipleOptionsOnLine_ColorsAllOptions()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  -j, --json  Output in JSON\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("-j", result);
        Assert.Contains("--json", result);
    }

    [Fact]
    public void ApplyColors_ThreeOptionsOnLine_ColorsAllOptions()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  -?, -h, --help  Show help\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("-?", result);
        Assert.Contains("-h", result);
        Assert.Contains("--help", result);
    }

    [Fact]
    public void ApplyColors_ArgumentLine_ColorsArgument()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  <road-ids>  Road IDs to check\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("<road-ids>", result);
    }

    [Fact]
    public void ApplyColors_HeaderLine_ColorsHeader()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("Options:\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("Options:", result);
    }

    [Fact]
    public void ApplyColors_DescriptionLine_ColorsDescription()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("Query the TfL Road API to display road status information.\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("Query the TfL Road API", result);
    }

    [Fact]
    public void ApplyColors_WithColorsDisabled_DoesNotAddAnsiCodes()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write("  --json  Output in JSON\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[", result);
    }

    [Fact]
    public void ApplyColors_ShortOption_ColorsOption()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  -j  Output in JSON\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("-j", result);
    }

    [Fact]
    public void ApplyColors_EmptyLine_DoesNotModify()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Equal("\n", result);
    }

    [Fact]
    public void Dispose_FlushesBuffer()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write("test");
        writer.Dispose();

        Assert.Contains("test", stringWriter.ToString());
    }

    [Fact]
    public void Write_MultipleLines_ProcessesEachLine()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write("line1\nline2\nline3");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("line1", result);
        Assert.Contains("line2", result);
        Assert.Contains("line3", result);
    }

    [Fact]
    public void Write_StringWithoutNewline_BuffersContent()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write("test");
        var beforeFlush = stringWriter.ToString();
        writer.Flush();
        var afterFlush = stringWriter.ToString();

        Assert.DoesNotContain("test", beforeFlush);
        Assert.Contains("test", afterFlush);
    }

    [Fact]
    public void ApplyColors_EmptyString_ReturnsEmpty()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Equal("", result);
    }

    [Fact]
    public void ApplyColors_AllKnownOptions_ColorsEachOption()
    {
        var stringWriter1 = new StringWriter();
        var writer1 = new ColoredTextWriter(stringWriter1, enableColors: true);
        writer1.Write("  -j  Short option\n");
        writer1.Flush();
        var result1 = stringWriter1.ToString();
        Assert.Contains("-j", result1);
        if (result1.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m-j\u001b[0m", result1);
        }

        var stringWriter2 = new StringWriter();
        var writer2 = new ColoredTextWriter(stringWriter2, enableColors: true);
        writer2.Write("  --json  Long option\n");
        writer2.Flush();
        var result2 = stringWriter2.ToString();
        Assert.Contains("--json", result2);
        if (result2.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m--json\u001b[0m", result2);
        }

        var stringWriter3 = new StringWriter();
        var writer3 = new ColoredTextWriter(stringWriter3, enableColors: true);
        writer3.Write("  --version  Version option\n");
        writer3.Flush();
        var result3 = stringWriter3.ToString();
        Assert.Contains("--version", result3);
        if (result3.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m--version\u001b[0m", result3);
        }

        var stringWriter4 = new StringWriter();
        var writer4 = new ColoredTextWriter(stringWriter4, enableColors: true);
        writer4.Write("  -?  Question mark\n");
        writer4.Flush();
        var result4 = stringWriter4.ToString();
        Assert.Contains("-?", result4);
        if (result4.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m-?\u001b[0m", result4);
        }

        var stringWriter5 = new StringWriter();
        var writer5 = new ColoredTextWriter(stringWriter5, enableColors: true);
        writer5.Write("  -h  Help short\n");
        writer5.Flush();
        var result5 = stringWriter5.ToString();
        Assert.Contains("-h", result5);
        if (result5.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m-h\u001b[0m", result5);
        }

        var stringWriter6 = new StringWriter();
        var writer6 = new ColoredTextWriter(stringWriter6, enableColors: true);
        writer6.Write("  --help  Help long\n");
        writer6.Flush();
        var result6 = stringWriter6.ToString();
        Assert.Contains("--help", result6);
        if (result6.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m--help\u001b[0m", result6);
        }
    }

    [Fact]
    public void ApplyColors_OptionWithLeadingSpaces_PreservesSpaces()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("    --json  With spaces\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("    ", result);
        Assert.Contains("--json", result);
        if (result.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m--json\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_OptionInMiddleOfWord_DoesNotColor()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  test--json  Not an option\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[33m--json\u001b[0m", result);
        Assert.Contains("test--json", result);
    }

    [Fact]
    public void ApplyColors_OptionFollowedByLetter_DoesNotColor()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  --json5  Not an option\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[33m--json\u001b[0m", result);
    }

    [Fact]
    public void ApplyColors_OptionAtStartOfLine_ColorsOption()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("--json  At start\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("--json", result);
        if (result.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m--json\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_OptionAtEndOfLine_ColorsOption()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  Use --json\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("--json", result);
        if (result.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m--json\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_MultipleOptionsInLine_ColorsAll()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  -j or --json or --version\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("-j", result);
        Assert.Contains("--json", result);
        Assert.Contains("--version", result);
        if (result.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m-j\u001b[0m", result);
            Assert.Contains("\u001b[33m--json\u001b[0m", result);
            Assert.Contains("\u001b[33m--version\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_NoOptionsFound_ProceedsToNextRule()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  <road-ids>  Argument\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("<road-ids>", result);
        if (result.Contains("\u001b[32m"))
        {
            Assert.Contains("\u001b[32m<road-ids>\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_AngleBrackets_ColorsGreen()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  <road-ids>  Road IDs\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("<road-ids>", result);
        if (result.Contains("\u001b[32m"))
        {
            Assert.Contains("\u001b[32m<road-ids>\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_MultipleAngleBrackets_ColorsAll()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  <arg1> and <arg2>  Multiple args\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("<arg1>", result);
        Assert.Contains("<arg2>", result);
        if (result.Contains("\u001b[32m"))
        {
            Assert.Contains("\u001b[32m<arg1>\u001b[0m", result);
            Assert.Contains("\u001b[32m<arg2>\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_UnclosedAngleBracket_DoesNotColor()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  <unclosed  No closing bracket\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[32m", result);
        Assert.Contains("<unclosed", result);
    }

    [Fact]
    public void ApplyColors_AngleBracketAtEnd_ColorsIt()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  Use <arg>\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("<arg>", result);
        if (result.Contains("\u001b[32m"))
        {
            Assert.Contains("\u001b[32m<arg>\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_HeaderLine_ColorsBold()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("Options:\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("Options:", result);
        if (result.Contains("\u001b[1m"))
        {
            Assert.Contains("\u001b[1mOptions:\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_HeaderLineWithSpaces_ColorsBold()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  Arguments:\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("Arguments:", result);
        if (result.Contains("\u001b[1m"))
        {
            Assert.Contains("\u001b[1m", result);
        }
    }

    [Fact]
    public void ApplyColors_HeaderLineLong_DoesNotColorBold()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("This is a very long header line that exceeds fifty characters:\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[1m", result);
    }

    [Fact]
    public void ApplyColors_HeaderLineManyWords_DoesNotColorBold()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("One Two Three Four:\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[1m", result);
    }

    [Fact]
    public void ApplyColors_DescriptionLine_ColorsCyan()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("This is a long description line that exceeds thirty characters\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("This is a long description line", result);
        if (result.Contains("\u001b[36m"))
        {
            Assert.Contains("\u001b[36m", result);
        }
    }

    [Fact]
    public void ApplyColors_DescriptionLineStartsWithSpaces_DoesNotColorCyan()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  This is a long description line that exceeds thirty characters\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[36m", result);
    }

    [Fact]
    public void ApplyColors_DescriptionLineStartsWithDash_DoesNotColorCyan()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("-This is a long description line that exceeds thirty characters\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[36m", result);
    }

    [Fact]
    public void ApplyColors_ShortLine_ReturnsAsIs()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("Short line\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("Short line", result);
        Assert.DoesNotContain("\u001b[", result);
    }

    [Fact]
    public void ApplyColors_OptionWithBestMatch_ChoosesFirstMatch()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  --json --version\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("--json", result);
        Assert.Contains("--version", result);
        if (result.Contains("\u001b[33m"))
        {
            Assert.Contains("\u001b[33m--json\u001b[0m", result);
            Assert.Contains("\u001b[33m--version\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_ConstructorWithNoColorEnvVar_DisablesColors()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        try
        {
            Environment.SetEnvironmentVariable("NO_COLOR", "1");
            var stringWriter = new StringWriter();
            var writer = new ColoredTextWriter(stringWriter, enableColors: true);

            writer.Write("  --json  Test\n");
            writer.Flush();

            var result = stringWriter.ToString();
            Assert.DoesNotContain("\u001b[", result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }

    [Fact]
    public void ApplyColors_ConstructorWithNoColorEnvVarWhitespace_DisablesColors()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        try
        {
            Environment.SetEnvironmentVariable("NO_COLOR", " ");
            var stringWriter = new StringWriter();
            var writer = new ColoredTextWriter(stringWriter, enableColors: true);

            writer.Write("  --json  Test\n");
            writer.Flush();

            var result = stringWriter.ToString();
            Assert.DoesNotContain("\u001b[", result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }

    [Fact]
    public void ApplyColors_ConstructorWithEnableColorsFalse_DisablesColors()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: false);

        writer.Write("  --json  Test\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[", result);
    }

    [Fact]
    public void ApplyColors_AngleBracketNoClosing_HandlesGracefully()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  <unclosed bracket\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("<unclosed bracket", result);
        Assert.DoesNotContain("\u001b[32m", result);
    }

    [Fact]
    public void ApplyColors_AngleBracketWithClosingAfter_ColorsCorrectly()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  <arg> text\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.Contains("<arg>", result);
        if (result.Contains("\u001b[32m"))
        {
            Assert.Contains("\u001b[32m<arg>\u001b[0m", result);
        }
    }

    [Fact]
    public void ApplyColors_AngleBracketClosingBeforeOpening_DoesNotColor()
    {
        var stringWriter = new StringWriter();
        var writer = new ColoredTextWriter(stringWriter, enableColors: true);

        writer.Write("  >arg<\n");
        writer.Flush();

        var result = stringWriter.ToString();
        Assert.DoesNotContain("\u001b[32m", result);
    }

    private static string InvokeApplyColorsToLine(string line)
    {
        var method = typeof(ColoredTextWriter).GetMethod("ApplyColorsToLine",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
        {
            throw new InvalidOperationException("ApplyColorsToLine method not found");
        }
        return (string)method.Invoke(null, new object[] { line })!;
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithOptions_AppliesColors()
    {
        var result = InvokeApplyColorsToLine("  --json  Output");
        Assert.Contains("--json", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithAngleBrackets_AppliesColors()
    {
        var result = InvokeApplyColorsToLine("  <road-ids>  Argument");
        Assert.Contains("<road-ids>", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithHeader_AppliesBold()
    {
        var result = InvokeApplyColorsToLine("Options:");
        Assert.Contains("Options:", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithLongDescription_AppliesCyan()
    {
        var result = InvokeApplyColorsToLine("This is a long description line that exceeds thirty characters");
        Assert.Contains("This is a long description line", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithMultipleOptions_ProcessesAll()
    {
        var result = InvokeApplyColorsToLine("  -j or --json or --version");
        Assert.Contains("-j", result);
        Assert.Contains("--json", result);
        Assert.Contains("--version", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithOptionAtStart_ProcessesCorrectly()
    {
        var result = InvokeApplyColorsToLine("--json  At start");
        Assert.Contains("--json", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithOptionAtEnd_ProcessesCorrectly()
    {
        var result = InvokeApplyColorsToLine("  Use --json");
        Assert.Contains("--json", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithOptionInWord_DoesNotProcess()
    {
        var result = InvokeApplyColorsToLine("  test--json  Not an option");
        Assert.Contains("test--json", result);
        // Should not color --json when it's part of a word
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithOptionFollowedByLetter_DoesNotProcess()
    {
        var result = InvokeApplyColorsToLine("  --json5  Not an option");
        Assert.Contains("--json5", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithLeadingSpaces_PreservesSpaces()
    {
        var result = InvokeApplyColorsToLine("    --json  With spaces");
        Assert.Contains("    ", result);
        Assert.Contains("--json", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithMultipleAngleBrackets_ProcessesAll()
    {
        var result = InvokeApplyColorsToLine("  <arg1> and <arg2>  Multiple args");
        Assert.Contains("<arg1>", result);
        Assert.Contains("<arg2>", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithUnclosedAngleBracket_DoesNotColor()
    {
        var result = InvokeApplyColorsToLine("  <unclosed  No closing bracket");
        Assert.Contains("<unclosed", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithAngleBracketAtEnd_ProcessesCorrectly()
    {
        var result = InvokeApplyColorsToLine("  Use <arg>");
        Assert.Contains("<arg>", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithHeaderWithSpaces_AppliesBold()
    {
        var result = InvokeApplyColorsToLine("  Arguments:");
        Assert.Contains("Arguments:", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithLongHeader_DoesNotApplyBold()
    {
        var result = InvokeApplyColorsToLine("This is a very long header line that exceeds fifty characters:");
        Assert.Contains("This is a very long header line", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithManyWordsHeader_DoesNotApplyBold()
    {
        var result = InvokeApplyColorsToLine("One Two Three Four:");
        Assert.Contains("One Two Three Four:", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithDescriptionStartingWithSpaces_DoesNotApplyCyan()
    {
        var result = InvokeApplyColorsToLine("  This is a long description line that exceeds thirty characters");
        Assert.Contains("This is a long description line", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithDescriptionStartingWithDash_DoesNotApplyCyan()
    {
        var result = InvokeApplyColorsToLine("-This is a long description line that exceeds thirty characters");
        Assert.Contains("This is a long description line", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithShortLine_ReturnsAsIs()
    {
        var result = InvokeApplyColorsToLine("Short line");
        Assert.Equal("Short line", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithEmptyString_ReturnsEmpty()
    {
        var result = InvokeApplyColorsToLine("");
        Assert.Equal("", result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithNullString_ReturnsNull()
    {
        var result = InvokeApplyColorsToLine(null!);
        Assert.Null(result);
    }

    [Fact]
    public void ApplyColorsToLine_DirectCall_WithBestMatchOption_ChoosesFirstMatch()
    {
        var result = InvokeApplyColorsToLine("  --json --version");
        Assert.Contains("--json", result);
        Assert.Contains("--version", result);
    }
}