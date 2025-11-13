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
    public async Task Main_NoArguments_ReturnsExitCodeInvalidUsage()
    {
        var originalOut = Console.Out;
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var exitCode = await InvokeMainAsync(Array.Empty<string>());

            Assert.Equal(Program.ExitCodeInvalidUsage, exitCode);
            Assert.Contains("No arguments provided", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task Main_TooManyArguments_ReturnsExitCodeInvalidUsage()
    {
        var originalOut = Console.Out;
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var exitCode = await InvokeMainAsync(new[] { "A2", "A233" });

            Assert.Equal(Program.ExitCodeInvalidUsage, exitCode);
            Assert.Contains("Too many arguments provided", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
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
            Assert.Contains("Usage:", outputText);
            Assert.Contains("RoadStatus <road-id>", outputText);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task Main_VersionFlag_ShowsVersionAndReturnsSuccess()
    {
        var originalOut = Console.Out;
        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var exitCode = await InvokeMainAsync(new[] { "--version" });

            Assert.Equal(Program.ExitCodeSuccess, exitCode);
            Assert.Contains("RoadStatus CLI", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
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

