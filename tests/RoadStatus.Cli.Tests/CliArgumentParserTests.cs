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
}

