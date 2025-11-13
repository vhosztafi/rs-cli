using Microsoft.Extensions.Logging;
using Serilog;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class LoggingConfigurationTests
{
    [Fact]
    public void ConfigureLogging_WithVerbose_SetsDebugLevel()
    {
        var loggerFactory = LoggingConfiguration.ConfigureLogging(verbose: true, quiet: false);
        
        Assert.NotNull(loggerFactory);
        var logger = loggerFactory.CreateLogger("Test");
        Assert.True(logger.IsEnabled(LogLevel.Debug));
        
        Log.CloseAndFlush();
    }

    [Fact]
    public void ConfigureLogging_WithQuiet_SetsErrorLevel()
    {
        var loggerFactory = LoggingConfiguration.ConfigureLogging(verbose: false, quiet: true);
        
        Assert.NotNull(loggerFactory);
        var logger = loggerFactory.CreateLogger("Test");
        Assert.False(logger.IsEnabled(LogLevel.Information));
        Assert.True(logger.IsEnabled(LogLevel.Error));
        
        Log.CloseAndFlush();
    }

    [Fact]
    public void ConfigureLogging_WithoutVerboseOrQuiet_SetsErrorLevel()
    {
        var loggerFactory = LoggingConfiguration.ConfigureLogging(verbose: false, quiet: false);
        
        Assert.NotNull(loggerFactory);
        var logger = loggerFactory.CreateLogger("Test");
        Assert.False(logger.IsEnabled(LogLevel.Information));
        Assert.True(logger.IsEnabled(LogLevel.Error));
        
        Log.CloseAndFlush();
    }

    [Fact]
    public void ConfigureLogging_ExtractsVersion()
    {
        var loggerFactory = LoggingConfiguration.ConfigureLogging(verbose: false, quiet: false);
        
        Assert.NotNull(loggerFactory);
        
        Log.CloseAndFlush();
    }

    [Fact]
    public void IsVerboseEnabled_WithCliVerbose_ReturnsTrue()
    {
        var result = LoggingConfiguration.IsVerboseEnabled(cliVerbose: true);
        
        Assert.True(result);
    }

    [Fact]
    public void IsVerboseEnabled_WithRoadStatusVerboseEnvVar1_ReturnsTrue()
    {
        var originalValue = Environment.GetEnvironmentVariable("ROADSTATUS_VERBOSE");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", "1");
            
            var result = LoggingConfiguration.IsVerboseEnabled(cliVerbose: false);
            
            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", originalValue);
        }
    }

    [Fact]
    public void IsVerboseEnabled_WithRoadStatusVerboseEnvVarTrue_ReturnsTrue()
    {
        var originalValue = Environment.GetEnvironmentVariable("ROADSTATUS_VERBOSE");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", "true");
            
            var result = LoggingConfiguration.IsVerboseEnabled(cliVerbose: false);
            
            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", originalValue);
        }
    }

    [Fact]
    public void IsVerboseEnabled_WithTflVerboseEnvVar1_ReturnsTrue()
    {
        var originalRoadStatus = Environment.GetEnvironmentVariable("ROADSTATUS_VERBOSE");
        var originalTfl = Environment.GetEnvironmentVariable("TFL_VERBOSE");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", null);
            Environment.SetEnvironmentVariable("TFL_VERBOSE", "1");
            
            var result = LoggingConfiguration.IsVerboseEnabled(cliVerbose: false);
            
            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", originalRoadStatus);
            Environment.SetEnvironmentVariable("TFL_VERBOSE", originalTfl);
        }
    }

    [Fact]
    public void IsVerboseEnabled_WithTflVerboseEnvVarTrue_ReturnsTrue()
    {
        var originalRoadStatus = Environment.GetEnvironmentVariable("ROADSTATUS_VERBOSE");
        var originalTfl = Environment.GetEnvironmentVariable("TFL_VERBOSE");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", null);
            Environment.SetEnvironmentVariable("TFL_VERBOSE", "true");
            
            var result = LoggingConfiguration.IsVerboseEnabled(cliVerbose: false);
            
            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", originalRoadStatus);
            Environment.SetEnvironmentVariable("TFL_VERBOSE", originalTfl);
        }
    }

    [Fact]
    public void IsVerboseEnabled_WithoutEnvVars_ReturnsFalse()
    {
        var originalRoadStatus = Environment.GetEnvironmentVariable("ROADSTATUS_VERBOSE");
        var originalTfl = Environment.GetEnvironmentVariable("TFL_VERBOSE");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", null);
            Environment.SetEnvironmentVariable("TFL_VERBOSE", null);
            
            var result = LoggingConfiguration.IsVerboseEnabled(cliVerbose: false);
            
            Assert.False(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", originalRoadStatus);
            Environment.SetEnvironmentVariable("TFL_VERBOSE", originalTfl);
        }
    }

    [Fact]
    public void IsVerboseEnabled_WithInvalidEnvVar_ReturnsFalse()
    {
        var originalRoadStatus = Environment.GetEnvironmentVariable("ROADSTATUS_VERBOSE");
        var originalTfl = Environment.GetEnvironmentVariable("TFL_VERBOSE");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", null);
            Environment.SetEnvironmentVariable("TFL_VERBOSE", "invalid");
            
            var result = LoggingConfiguration.IsVerboseEnabled(cliVerbose: false);
            
            Assert.False(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_VERBOSE", originalRoadStatus);
            Environment.SetEnvironmentVariable("TFL_VERBOSE", originalTfl);
        }
    }

    [Fact]
    public void IsQuietEnabled_WithCliQuiet_ReturnsTrue()
    {
        var result = LoggingConfiguration.IsQuietEnabled(cliQuiet: true);
        
        Assert.True(result);
    }

    [Fact]
    public void IsQuietEnabled_WithRoadStatusQuietEnvVar1_ReturnsTrue()
    {
        var originalValue = Environment.GetEnvironmentVariable("ROADSTATUS_QUIET");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", "1");
            
            var result = LoggingConfiguration.IsQuietEnabled(cliQuiet: false);
            
            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", originalValue);
        }
    }

    [Fact]
    public void IsQuietEnabled_WithRoadStatusQuietEnvVarTrue_ReturnsTrue()
    {
        var originalValue = Environment.GetEnvironmentVariable("ROADSTATUS_QUIET");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", "true");
            
            var result = LoggingConfiguration.IsQuietEnabled(cliQuiet: false);
            
            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", originalValue);
        }
    }

    [Fact]
    public void IsQuietEnabled_WithTflQuietEnvVar1_ReturnsTrue()
    {
        var originalRoadStatus = Environment.GetEnvironmentVariable("ROADSTATUS_QUIET");
        var originalTfl = Environment.GetEnvironmentVariable("TFL_QUIET");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", null);
            Environment.SetEnvironmentVariable("TFL_QUIET", "1");
            
            var result = LoggingConfiguration.IsQuietEnabled(cliQuiet: false);
            
            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", originalRoadStatus);
            Environment.SetEnvironmentVariable("TFL_QUIET", originalTfl);
        }
    }

    [Fact]
    public void IsQuietEnabled_WithTflQuietEnvVarTrue_ReturnsTrue()
    {
        var originalRoadStatus = Environment.GetEnvironmentVariable("ROADSTATUS_QUIET");
        var originalTfl = Environment.GetEnvironmentVariable("TFL_QUIET");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", null);
            Environment.SetEnvironmentVariable("TFL_QUIET", "true");
            
            var result = LoggingConfiguration.IsQuietEnabled(cliQuiet: false);
            
            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", originalRoadStatus);
            Environment.SetEnvironmentVariable("TFL_QUIET", originalTfl);
        }
    }

    [Fact]
    public void IsQuietEnabled_WithoutEnvVars_ReturnsFalse()
    {
        var originalRoadStatus = Environment.GetEnvironmentVariable("ROADSTATUS_QUIET");
        var originalTfl = Environment.GetEnvironmentVariable("TFL_QUIET");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", null);
            Environment.SetEnvironmentVariable("TFL_QUIET", null);
            
            var result = LoggingConfiguration.IsQuietEnabled(cliQuiet: false);
            
            Assert.False(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", originalRoadStatus);
            Environment.SetEnvironmentVariable("TFL_QUIET", originalTfl);
        }
    }

    [Fact]
    public void IsQuietEnabled_WithInvalidEnvVar_ReturnsFalse()
    {
        var originalRoadStatus = Environment.GetEnvironmentVariable("ROADSTATUS_QUIET");
        var originalTfl = Environment.GetEnvironmentVariable("TFL_QUIET");
        try
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", null);
            Environment.SetEnvironmentVariable("TFL_QUIET", "invalid");
            
            var result = LoggingConfiguration.IsQuietEnabled(cliQuiet: false);
            
            Assert.False(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ROADSTATUS_QUIET", originalRoadStatus);
            Environment.SetEnvironmentVariable("TFL_QUIET", originalTfl);
        }
    }
}

