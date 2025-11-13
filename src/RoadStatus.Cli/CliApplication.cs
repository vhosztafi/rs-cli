using Microsoft.Extensions.Logging;
using RoadStatus.Core;

namespace RoadStatus.Cli;

public class CliApplication
{
    private readonly ITflRoadStatusClient _client;
    private readonly RoadStatusFormatter _formatter;
    private readonly ILogger<CliApplication> _logger;

    public CliApplication(ITflRoadStatusClient client, RoadStatusFormatter formatter, ILogger<CliApplication> logger)
    {
        _client = client;
        _formatter = formatter;
        _logger = logger;
    }

    public async Task<int> RunAsync(string[] roadIds, bool jsonOutput, TextWriter output)
    {
        var startTime = DateTime.UtcNow;
        var correlationId = Guid.NewGuid().ToString("N")[..16]; // Short correlation ID

        if (roadIds.Length == 0)
        {
            _logger.LogWarning("No road IDs provided {CorrelationId}", correlationId);
            await output.WriteLineAsync("At least one road ID is required.");
            return Program.ExitCodeInvalidUsage;
        }

        _logger.LogInformation("Starting road status request {CorrelationId} for {RoadIds}", correlationId, string.Join(", ", roadIds));

        var roadStatuses = new List<Core.RoadStatus>();
        var errors = new List<string>();
        var hasInvalidRoad = false;

        foreach (var roadIdString in roadIds)
        {
            try
            {
                _logger.LogDebug("Fetching status for road {CorrelationId} {RoadId}", correlationId, roadIdString);
                var roadId = RoadId.Parse(roadIdString);
                var roadStatus = await _client.GetRoadStatusAsync(roadId);
                roadStatuses.Add(roadStatus);

                _logger.LogInformation(
                    "Road status retrieved successfully {CorrelationId} {RoadId} {DisplayName} {StatusSeverity}",
                    correlationId,
                    roadIdString,
                    roadStatus.DisplayName,
                    roadStatus.StatusSeverity);
            }
            catch (UnknownRoadException ex)
            {
                _logger.LogWarning("Road not found {CorrelationId} {RoadId}", correlationId, roadIdString);
                errors.Add(ex.Message);
                hasInvalidRoad = true;
            }
            catch (RoadStatusException ex)
            {
                var executionTime = DateTime.UtcNow - startTime;
                _logger.LogError(
                    ex,
                    "Error while fetching road status {CorrelationId} {RoadId} {ExecutionTimeMs}ms {ErrorType}",
                    correlationId,
                    roadIdString,
                    executionTime.TotalMilliseconds,
                    ex.GetType().Name);
                errors.Add(ex.Message);
                hasInvalidRoad = true;
            }
            catch (Exception ex)
            {
                var executionTime = DateTime.UtcNow - startTime;
                _logger.LogError(
                    ex,
                    "Unexpected error while fetching road status {CorrelationId} {RoadId} {ExecutionTimeMs}ms {ErrorType}",
                    correlationId,
                    roadIdString,
                    executionTime.TotalMilliseconds,
                    ex.GetType().Name);
                errors.Add($"An unexpected error occurred while retrieving status for {roadIdString}: {ex.Message}");
                hasInvalidRoad = true;
            }
        }

        if (jsonOutput)
        {
            var jsonOutputText = _formatter.FormatJson(roadStatuses);
            await output.WriteLineAsync(jsonOutputText);
        }
        else
        {
            foreach (var roadStatus in roadStatuses)
            {
                var formattedOutput = _formatter.Format(roadStatus);
                await output.WriteAsync(formattedOutput);
            }
        }

        foreach (var error in errors)
        {
            await output.WriteLineAsync(error);
        }

        var finalExecutionTime = DateTime.UtcNow - startTime;
        var exitCode = hasInvalidRoad ? Program.ExitCodeInvalidRoad : Program.ExitCodeSuccess;

        _logger.LogInformation(
            "Road status request completed {CorrelationId} {RoadIds} {ExecutionTimeMs}ms {ExitCode}",
            correlationId,
            string.Join(", ", roadIds),
            finalExecutionTime.TotalMilliseconds,
            exitCode);

        return exitCode;
    }
}
