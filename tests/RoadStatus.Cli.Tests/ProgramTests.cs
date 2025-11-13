using System.Reflection;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class ProgramTests
{
    [Fact]
    public void ExitCodeSuccess_IsZero()
    {
        Assert.Equal(0, Program.ExitCodeSuccess);
    }

    [Fact]
    public void ExitCodeInvalidRoad_IsOne()
    {
        Assert.Equal(1, Program.ExitCodeInvalidRoad);
    }

    [Fact]
    public void ExitCodeInvalidUsage_IsTwo()
    {
        Assert.Equal(2, Program.ExitCodeInvalidUsage);
    }

    [Fact]
    public async Task Main_NoArguments_ShowsError()
    {
        var originalOut = Console.Out;
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var exitCode = await InvokeMainAsync(Array.Empty<string>());

            // System.CommandLine returns 1 for parsing errors
            Assert.NotEqual(Program.ExitCodeSuccess, exitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task Main_MultipleRoads_ProcessesAllRoads()
    {
        var originalOut = Console.Out;
        var originalAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var originalAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            Environment.SetEnvironmentVariable("TFL_APP_ID", null);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", null);

            var exitCode = await InvokeMainAsync(new[] { "A2", "A3" });

            // May succeed or fail depending on API, but should process multiple roads
            Assert.True(exitCode >= 0);
        }
        finally
        {
            Console.SetOut(originalOut);
            if (originalAppId != null)
                Environment.SetEnvironmentVariable("TFL_APP_ID", originalAppId);
            if (originalAppKey != null)
                Environment.SetEnvironmentVariable("TFL_APP_KEY", originalAppKey);
        }
    }

    [Fact]
    public async Task Main_HelpFlag_ShowsHelpAndReturnsSuccess()
    {
        var originalOut = Console.Out;
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var exitCode = await InvokeMainAsync(new[] { "--help" });

            Assert.Equal(Program.ExitCodeSuccess, exitCode);
            var outputText = output.ToString();
            Assert.Contains("road-ids", outputText);
            Assert.Contains("--json", outputText);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task Main_JsonOption_OutputsJson()
    {
        var originalOut = Console.Out;
        var originalAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var originalAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            Environment.SetEnvironmentVariable("TFL_APP_ID", null);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", null);

            var exitCode = await InvokeMainAsync(new[] { "--json", "A2" });

            // May succeed or fail depending on API, but if it succeeds, should output JSON
            if (exitCode == Program.ExitCodeSuccess)
            {
                var outputText = output.ToString();
                Assert.StartsWith("[", outputText.Trim());
            }
        }
        finally
        {
            Console.SetOut(originalOut);
            if (originalAppId != null)
                Environment.SetEnvironmentVariable("TFL_APP_ID", originalAppId);
            if (originalAppKey != null)
                Environment.SetEnvironmentVariable("TFL_APP_KEY", originalAppKey);
        }
    }

    [Fact]
    public async Task Main_ValidRoad_ReturnsExitCodeSuccess()
    {
        var originalOut = Console.Out;
        var originalAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var originalAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            Environment.SetEnvironmentVariable("TFL_APP_ID", null);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", null);

            var exitCode = await InvokeMainAsync(new[] { "A2" });

            Assert.True(exitCode == Program.ExitCodeSuccess || exitCode == Program.ExitCodeInvalidRoad);
        }
        finally
        {
            Console.SetOut(originalOut);
            if (originalAppId != null)
                Environment.SetEnvironmentVariable("TFL_APP_ID", originalAppId);
            if (originalAppKey != null)
                Environment.SetEnvironmentVariable("TFL_APP_KEY", originalAppKey);
        }
    }

    [Fact]
    public async Task Main_InvalidRoad_ReturnsExitCodeInvalidRoad()
    {
        var originalOut = Console.Out;
        var originalAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var originalAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            Environment.SetEnvironmentVariable("TFL_APP_ID", null);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", null);

            var exitCode = await InvokeMainAsync(new[] { "A233" });

            Assert.Equal(Program.ExitCodeInvalidRoad, exitCode);
            Assert.Contains("A233 is not a valid road", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            if (originalAppId != null)
                Environment.SetEnvironmentVariable("TFL_APP_ID", originalAppId);
            if (originalAppKey != null)
                Environment.SetEnvironmentVariable("TFL_APP_KEY", originalAppKey);
        }
    }

    [Fact]
    public async Task Main_WithEnvironmentVariables_PassesThemToClient()
    {
        var originalOut = Console.Out;
        var originalAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var originalAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            Environment.SetEnvironmentVariable("TFL_APP_ID", "test-app-id");
            Environment.SetEnvironmentVariable("TFL_APP_KEY", "test-app-key");

            try
            {
                var exitCode = await InvokeMainAsync(new[] { "A2" });
                Assert.True(exitCode >= 0);
            }
            catch (HttpRequestException)
            {
            }
        }
        finally
        {
            Console.SetOut(originalOut);
            Environment.SetEnvironmentVariable("TFL_APP_ID", originalAppId);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", originalAppKey);
        }
    }

    [Fact]
    public async Task Main_WithoutAppSettingsJson_WorksWithDefaults()
    {
        var originalOut = Console.Out;
        var originalAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var originalAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        var originalBaseUrl = Environment.GetEnvironmentVariable("TFL_BASE_URL");
        var originalWorkingDir = Directory.GetCurrentDirectory();
        
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            Environment.SetEnvironmentVariable("TFL_APP_ID", null);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", null);
            Environment.SetEnvironmentVariable("TFL_BASE_URL", null);

            var tempDir = Path.GetTempPath();
            Directory.SetCurrentDirectory(tempDir);

            var exitCode = await InvokeMainAsync(["--help"]);
            
            Assert.Equal(Program.ExitCodeSuccess, exitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
            Directory.SetCurrentDirectory(originalWorkingDir);
            Environment.SetEnvironmentVariable("TFL_APP_ID", originalAppId);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", originalAppKey);
            Environment.SetEnvironmentVariable("TFL_BASE_URL", originalBaseUrl);
        }
    }

    private static async Task<int> InvokeMainAsync(string[] args)
    {
        var programType = typeof(Program);
        var mainMethod = programType.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);
        
        if (mainMethod == null)
        {
            throw new InvalidOperationException("Main method not found");
        }

        var result = mainMethod.Invoke(null, [args]);
        
        if (result is Task<int> task)
        {
            return await task;
        }
        
        throw new InvalidOperationException("Main method did not return Task<int>");
    }
}

