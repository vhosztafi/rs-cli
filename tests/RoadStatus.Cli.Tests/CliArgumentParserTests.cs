using RoadStatus.Cli;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class CliArgumentParserTests
{
    [Fact]
    public void Parse_NoArguments_ReturnsInvalidUsage()
    {
        var parser = new CliArgumentParser();
        var args = Array.Empty<string>();

        var result = parser.Parse(args);

        Assert.False(result.IsSuccess);
        Assert.Null(result.RoadId);
        Assert.Equal("No arguments provided.", result.ErrorMessage);
    }

    [Fact]
    public void Parse_TooManyArguments_ReturnsInvalidUsage()
    {
        var parser = new CliArgumentParser();
        var args = new[] { "A2", "A233" };

        var result = parser.Parse(args);

        Assert.False(result.IsSuccess);
        Assert.Null(result.RoadId);
        Assert.Equal("Too many arguments provided.", result.ErrorMessage);
    }

    [Fact]
    public void Parse_SingleArgument_ReturnsSuccess()
    {
        var parser = new CliArgumentParser();
        var args = new[] { "A2" };

        var result = parser.Parse(args);

        Assert.True(result.IsSuccess);
        Assert.Equal("A2", result.RoadId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Parse_HelpFlag_ReturnsShowHelp()
    {
        var parser = new CliArgumentParser();
        var args = new[] { "--help" };

        var result = parser.Parse(args);

        Assert.False(result.IsSuccess);
        Assert.True(result.ShouldShowHelp);
        Assert.False(result.ShouldShowVersion);
        Assert.Null(result.RoadId);
    }

    [Fact]
    public void Parse_ShortHelpFlag_ReturnsShowHelp()
    {
        var parser = new CliArgumentParser();
        var args = new[] { "-h" };

        var result = parser.Parse(args);

        Assert.True(result.ShouldShowHelp);
    }

    [Fact]
    public void Parse_VersionFlag_ReturnsShowVersion()
    {
        var parser = new CliArgumentParser();
        var args = new[] { "--version" };

        var result = parser.Parse(args);

        Assert.False(result.IsSuccess);
        Assert.False(result.ShouldShowHelp);
        Assert.True(result.ShouldShowVersion);
        Assert.Null(result.RoadId);
    }

    [Fact]
    public void Parse_ShortVersionFlag_ReturnsShowVersion()
    {
        var parser = new CliArgumentParser();
        var args = new[] { "-v" };

        var result = parser.Parse(args);

        Assert.True(result.ShouldShowVersion);
    }
}

