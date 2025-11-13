using System.Diagnostics;
using System.Text;
using Reqnroll;
using Xunit;

namespace RoadStatus.Specs.StepDefinitions;

[Binding]
public class RoadStatusSteps
{
    private int _exitCode;
    private string _output = string.Empty;
    private string _errorOutput = string.Empty;

    [Given("I want to query the road status")]
    public void GivenIWantToQueryTheRoadStatus()
    {
    }

    [When(@"I run the CLI with road ID ""(.*)""")]
    public void WhenIRunTheCliWithRoadId(string roadId)
    {
        var solutionDirectory = GetSolutionDirectory();
        var cliProjectPath = Path.Combine(solutionDirectory, "src", "RoadStatus.Cli", "RoadStatus.Cli.csproj");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{cliProjectPath}\" -- \"{roadId}\"",
                WorkingDirectory = solutionDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        _exitCode = process.ExitCode;
        _output = outputBuilder.ToString();
        _errorOutput = errorBuilder.ToString();
    }

    [Then("the exit code should be (.*)")]
    public void ThenTheExitCodeShouldBe(int expectedExitCode)
    {
        Assert.Equal(expectedExitCode, _exitCode);
    }

    [Then(@"the output should contain ""(.*)""")]
    public void ThenTheOutputShouldContain(string expectedText)
    {
        var fullOutput = _output + _errorOutput;
        Assert.Contains(expectedText, fullOutput);
    }

    private static string GetSolutionDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDirectory);

        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "rs-cli.sln")))
        {
            directory = directory.Parent;
        }

        if (directory == null)
        {
            throw new InvalidOperationException("Solution directory not found");
        }

        return directory.FullName;
    }
}