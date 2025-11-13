using System.Text;
using RoadStatus.Cli;
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

        var exitCode = await app.RunAsync(new[] { "A2" }, output);

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
