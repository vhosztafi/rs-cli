using Xunit;

namespace RoadStatus.Cli.Tests;

public class ConsoleColorsTests
{
    [Fact]
    public void Cyan_WithText_ReturnsText()
    {
        var result = ConsoleColors.Cyan("test");
        
        Assert.Contains("test", result);
    }

    [Fact]
    public void Yellow_WithText_ReturnsText()
    {
        var result = ConsoleColors.Yellow("test");
        
        Assert.Contains("test", result);
    }

    [Fact]
    public void Green_WithText_ReturnsText()
    {
        var result = ConsoleColors.Green("test");
        
        Assert.Contains("test", result);
    }

    [Fact]
    public void Bold_WithText_ReturnsText()
    {
        var result = ConsoleColors.Bold("test");
        
        Assert.Contains("test", result);
    }

    [Fact]
    public void Reset_ReturnsString()
    {
        var result = ConsoleColors.Reset;
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Cyan_WithEmptyString_ReturnsEmptyString()
    {
        var result = ConsoleColors.Cyan("");
        
        Assert.Equal("", result);
    }

    [Fact]
    public void Yellow_WithEmptyString_ReturnsEmptyString()
    {
        var result = ConsoleColors.Yellow("");
        
        Assert.Equal("", result);
    }

    [Fact]
    public void Green_WithEmptyString_ReturnsEmptyString()
    {
        var result = ConsoleColors.Green("");
        
        Assert.Equal("", result);
    }

    [Fact]
    public void Bold_WithEmptyString_ReturnsEmptyString()
    {
        var result = ConsoleColors.Bold("");
        
        Assert.Equal("", result);
    }

    [Fact]
    public void Cyan_WithNull_ReturnsNull()
    {
        var result = ConsoleColors.Cyan(null!);
        
        Assert.Null(result);
    }

    [Fact]
    public void Yellow_WithNull_ReturnsNull()
    {
        var result = ConsoleColors.Yellow(null!);
        
        Assert.Null(result);
    }

    [Fact]
    public void Green_WithNull_ReturnsNull()
    {
        var result = ConsoleColors.Green(null!);
        
        Assert.Null(result);
    }

    [Fact]
    public void Bold_WithNull_ReturnsNull()
    {
        var result = ConsoleColors.Bold(null!);
        
        Assert.Null(result);
    }

    [Fact]
    public void Cyan_PreservesInputText()
    {
        var input = "sample text";
        var result = ConsoleColors.Cyan(input);
        
        Assert.Contains(input, result);
    }

    [Fact]
    public void Yellow_PreservesInputText()
    {
        var input = "sample text";
        var result = ConsoleColors.Yellow(input);
        
        Assert.Contains(input, result);
    }

    [Fact]
    public void Green_PreservesInputText()
    {
        var input = "sample text";
        var result = ConsoleColors.Green(input);
        
        Assert.Contains(input, result);
    }

    [Fact]
    public void Bold_PreservesInputText()
    {
        var input = "sample text";
        var result = ConsoleColors.Bold(input);
        
        Assert.Contains(input, result);
    }
}