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
}