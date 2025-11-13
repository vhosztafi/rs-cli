using System.Reflection;
using RoadStatus.Core;
using CoreRoadStatus = RoadStatus.Core.RoadStatus;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class CliApplicationTests
{
    [Fact]
    public async Task RunAsync_NoArguments_ReturnsExitCodeInvalidUsage()
    {
        var parser = new CliArgumentParser();
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var app = new CliApplication(parser, mockClient, formatter);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(Array.Empty<string>(), output);

        Assert.Equal(2, exitCode);
        Assert.Contains("No arguments provided", output.ToString());
    }

    [Fact]
    public async Task RunAsync_TooManyArguments_ReturnsExitCodeInvalidUsage()
    {
        var parser = new CliArgumentParser();
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var app = new CliApplication(parser, mockClient, formatter);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A2", "A233" }, output);

        Assert.Equal(2, exitCode);
        Assert.Contains("Too many arguments provided", output.ToString());
    }

    [Fact]
    public async Task RunAsync_ValidRoad_ReturnsExitCodeSuccess()
    {
        var parser = new CliArgumentParser();
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var app = new CliApplication(parser, mockClient, formatter);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(["A2"], output);

        Assert.Equal(0, exitCode);
        var outputText = output.ToString();
        Assert.Contains("The status of the A2 is as follows", outputText);
        Assert.Contains("Road Status is Good", outputText);
        Assert.Contains("Road Status Description is No Exceptional Delays", outputText);
    }

    [Fact]
    public async Task RunAsync_InvalidRoad_ReturnsExitCodeInvalidRoad()
    {
        var parser = new CliArgumentParser();
        var mockClient = new MockTflRoadStatusClient(shouldThrowNotFound: true);
        var formatter = new RoadStatusFormatter();
        var app = new CliApplication(parser, mockClient, formatter);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A233" }, output);

        Assert.Equal(1, exitCode);
        Assert.Contains("A233 is not a valid road", output.ToString());
    }

    [Fact]
    public async Task RunAsync_HelpFlag_ShowsHelpAndReturnsSuccess()
    {
        var parser = new CliArgumentParser();
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var app = new CliApplication(parser, mockClient, formatter);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "--help" }, output);

        Assert.Equal(0, exitCode);
        var outputText = output.ToString();
        Assert.Contains("Usage:", outputText);
        Assert.Contains("RoadStatus <road-id>", outputText);
    }

    [Fact]
    public async Task RunAsync_VersionFlag_ShowsVersionAndReturnsSuccess()
    {
        var parser = new CliArgumentParser();
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var app = new CliApplication(parser, mockClient, formatter);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(["--version"], output);

        Assert.Equal(0, exitCode);
        var outputText = output.ToString();
        Assert.Contains("RoadStatus CLI", outputText);
    }

    [Fact]
    public async Task RunAsync_VersionFlag_WithNullVersion_ShowsVersionWithZero()
    {
        var parser = new CliArgumentParser();
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var app = new TestableCliApplicationWithNullVersion(parser, mockClient, formatter);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(["--version"], output);

        Assert.Equal(0, exitCode);
        var outputText = output.ToString();
        Assert.Contains("RoadStatus CLI", outputText);
        Assert.Contains("0", outputText);
    }

    private sealed class TestableCliApplicationWithNullVersion : CliApplication
    {
        public TestableCliApplicationWithNullVersion(CliArgumentParser parser, ITflRoadStatusClient client, RoadStatusFormatter formatter)
            : base(parser, client, formatter)
        {
        }

        protected override Version? GetVersion()
        {
            return null;
        }
    }

    [Fact]
    public async Task RunAsync_NullRoadId_ReturnsExitCodeInvalidUsage()
    {
        var resultType = typeof(CliArgumentParseResult);
        var constructor = resultType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
        var parseResultWithNullRoadId = (CliArgumentParseResult)constructor.Invoke(new object?[] { true, false, false, null, null });
        
        var mockParser = new MockParserWithNullRoadId(parseResultWithNullRoadId);
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var app = new CliApplication(mockParser, mockClient, formatter);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A2" }, output);

        Assert.Equal(2, exitCode);
        Assert.Contains("Road ID is required.", output.ToString());
    }

    private sealed class MockParserWithNullRoadId : CliArgumentParser
    {
        private readonly CliArgumentParseResult _result;

        public MockParserWithNullRoadId(CliArgumentParseResult result)
        {
            _result = result;
        }

        public override CliArgumentParseResult Parse(string[] args) => _result;
    }

    private sealed class MockTflRoadStatusClient : ITflRoadStatusClient
    {
        private readonly bool _shouldThrowNotFound;

        public MockTflRoadStatusClient(bool shouldThrowNotFound = false)
        {
            _shouldThrowNotFound = shouldThrowNotFound;
        }

        public Task<CoreRoadStatus> GetRoadStatusAsync(RoadId roadId)
        {
            if (_shouldThrowNotFound)
            {
                throw new UnknownRoadException(roadId.ToString());
            }

            return Task.FromResult(new CoreRoadStatus(
                "A2",
                "Good",
                "No Exceptional Delays"));
        }
    }
}