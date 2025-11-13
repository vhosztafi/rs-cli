using Microsoft.Extensions.Logging.Abstractions;
using RoadStatus.Core;
using CoreRoadStatus = RoadStatus.Core.RoadStatus;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class CliApplicationTests
{
    [Fact]
    public async Task RunAsync_NoRoadIds_ReturnsExitCodeInvalidUsage()
    {
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(Array.Empty<string>(), false, output);

        Assert.Equal(2, exitCode);
        Assert.Contains("At least one road ID is required", output.ToString());
    }

    [Fact]
    public async Task RunAsync_ValidRoad_ReturnsExitCodeSuccess()
    {
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(["A2"], false, output);

        Assert.Equal(0, exitCode);
        var outputText = output.ToString();
        Assert.Contains("The status of the A2 is as follows", outputText);
        Assert.Contains("Road Status is Good", outputText);
        Assert.Contains("Road Status Description is No Exceptional Delays", outputText);
    }

    [Fact]
    public async Task RunAsync_InvalidRoad_ReturnsExitCodeInvalidRoad()
    {
        var mockClient = new MockTflRoadStatusClient(shouldThrowNotFound: true);
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A233" }, false, output);

        Assert.Equal(1, exitCode);
        Assert.Contains("A233 is not a valid road", output.ToString());
    }

    [Fact]
    public async Task RunAsync_MultipleValidRoads_ReturnsExitCodeSuccess()
    {
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A2", "A3" }, false, output);

        Assert.Equal(0, exitCode);
        var outputText = output.ToString();
        Assert.Contains("The status of the A2 is as follows", outputText);
        Assert.Contains("The status of the A3 is as follows", outputText);
    }

    [Fact]
    public async Task RunAsync_MultipleRoads_OneInvalid_ReturnsExitCodeInvalidRoad()
    {
        var mockClient = new MockTflRoadStatusClient(shouldThrowNotFound: true);
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A2", "A233" }, false, output);

        Assert.Equal(1, exitCode);
        var outputText = output.ToString();
        Assert.Contains("A233 is not a valid road", outputText);
    }

    [Fact]
    public async Task RunAsync_ValidRoad_JsonOutput_ReturnsJson()
    {
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(["A2"], true, output);

        Assert.Equal(0, exitCode);
        var outputText = output.ToString();
        Assert.Contains("\"displayName\":\"A2\"", outputText);
        Assert.Contains("\"statusSeverity\":\"Good\"", outputText);
        Assert.Contains("\"statusDescription\":\"No Exceptional Delays\"", outputText);
        Assert.StartsWith("[", outputText);
        var trimmed = outputText.TrimEnd();
        Assert.EndsWith("]", trimmed);
    }

    [Fact]
    public async Task RunAsync_MultipleValidRoads_JsonOutput_ReturnsJsonArray()
    {
        var mockClient = new MockTflRoadStatusClient();
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A2", "A3" }, true, output);

        Assert.Equal(0, exitCode);
        var outputText = output.ToString();
        Assert.StartsWith("[", outputText);
        var trimmed = outputText.TrimEnd();
        Assert.EndsWith("]", trimmed);
        Assert.Contains("\"displayName\":\"A2\"", outputText);
        Assert.Contains("\"displayName\":\"A3\"", outputText);
    }

    [Fact]
    public async Task RunAsync_MultipleRoads_MixedValidInvalid_ShowsBothResultsAndErrors()
    {
        var mockClient = new MockTflRoadStatusClient(invalidRoadIds: new[] { "A233" });
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A2", "A233" }, false, output);

        Assert.Equal(1, exitCode);
        var outputText = output.ToString();
        Assert.Contains("The status of the A2 is as follows", outputText);
        Assert.Contains("A233 is not a valid road", outputText);
    }

    [Fact]
    public async Task RunAsync_AllInvalidRoads_JsonOutput_ReturnsEmptyArray()
    {
        var mockClient = new MockTflRoadStatusClient(shouldThrowNotFound: true);
        var formatter = new RoadStatusFormatter();
        var logger = NullLogger<CliApplication>.Instance;
        var app = new CliApplication(mockClient, formatter, logger);
        var output = new StringWriter();

        var exitCode = await app.RunAsync(new[] { "A233" }, true, output);

        Assert.Equal(1, exitCode);
        var outputText = output.ToString();
        Assert.StartsWith("[", outputText.Trim());
        Assert.Contains("]", outputText);
        Assert.Contains("A233 is not a valid road", outputText);
    }

    private sealed class MockTflRoadStatusClient : ITflRoadStatusClient
    {
        private readonly bool _shouldThrowNotFound;
        private readonly HashSet<string> _invalidRoadIds;

        public MockTflRoadStatusClient(bool shouldThrowNotFound = false, string[]? invalidRoadIds = null)
        {
            _shouldThrowNotFound = shouldThrowNotFound;
            _invalidRoadIds = invalidRoadIds != null ? new HashSet<string>(invalidRoadIds) : new HashSet<string>();
        }

        public Task<CoreRoadStatus> GetRoadStatusAsync(RoadId roadId)
        {
            var roadIdString = roadId.ToString();
            if (_shouldThrowNotFound || _invalidRoadIds.Contains(roadIdString))
            {
                throw new UnknownRoadException(roadIdString);
            }

            // Return different statuses for different roads to test multiple roads
            var displayName = roadIdString;
            return Task.FromResult(new CoreRoadStatus(
                displayName,
                "Good",
                "No Exceptional Delays"));
        }
    }
}