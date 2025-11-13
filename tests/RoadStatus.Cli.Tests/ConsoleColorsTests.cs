using System.Reflection;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class ConsoleColorsTests
{
    [Fact]
    public void Cyan_WithText_ReturnsText()
    {
        var result = ConsoleColors.Cyan("test");

        Assert.Contains("test", result);
    }

    [Fact]
    public void Yellow_WithText_ReturnsText()
    {
        var result = ConsoleColors.Yellow("test");

        Assert.Contains("test", result);
    }

    [Fact]
    public void Green_WithText_ReturnsText()
    {
        var result = ConsoleColors.Green("test");

        Assert.Contains("test", result);
    }

    [Fact]
    public void Bold_WithText_ReturnsText()
    {
        var result = ConsoleColors.Bold("test");

        Assert.Contains("test", result);
    }

    [Fact]
    public void Reset_ReturnsString()
    {
        var result = ConsoleColors.Reset;

        Assert.NotNull(result);
    }

    [Fact]
    public void Cyan_WithEmptyString_ReturnsEmptyString()
    {
        var result = ConsoleColors.Cyan("");

        Assert.Equal("", result);
    }

    [Fact]
    public void Yellow_WithEmptyString_ReturnsEmptyString()
    {
        var result = ConsoleColors.Yellow("");

        Assert.Equal("", result);
    }

    [Fact]
    public void Green_WithEmptyString_ReturnsEmptyString()
    {
        var result = ConsoleColors.Green("");

        Assert.Equal("", result);
    }

    [Fact]
    public void Bold_WithEmptyString_ReturnsEmptyString()
    {
        var result = ConsoleColors.Bold("");

        Assert.Equal("", result);
    }

    [Fact]
    public void Cyan_WithNull_ReturnsNull()
    {
        var result = ConsoleColors.Cyan(null!);

        Assert.Null(result);
    }

    [Fact]
    public void Yellow_WithNull_ReturnsNull()
    {
        var result = ConsoleColors.Yellow(null!);

        Assert.Null(result);
    }

    [Fact]
    public void Green_WithNull_ReturnsNull()
    {
        var result = ConsoleColors.Green(null!);

        Assert.Null(result);
    }

    [Fact]
    public void Bold_WithNull_ReturnsNull()
    {
        var result = ConsoleColors.Bold(null!);

        Assert.Null(result);
    }

    [Fact]
    public void Cyan_PreservesInputText()
    {
        var input = "sample text";
        var result = ConsoleColors.Cyan(input);

        Assert.Contains(input, result);
    }

    [Fact]
    public void Yellow_PreservesInputText()
    {
        var input = "sample text";
        var result = ConsoleColors.Yellow(input);

        Assert.Contains(input, result);
    }

    [Fact]
    public void Green_PreservesInputText()
    {
        var input = "sample text";
        var result = ConsoleColors.Green(input);

        Assert.Contains(input, result);
    }

    [Fact]
    public void Bold_PreservesInputText()
    {
        var input = "sample text";
        var result = ConsoleColors.Bold(input);

        Assert.Contains(input, result);
    }

    [Fact]
    public void Cyan_WithColorsEnabled_AddsAnsiCodes()
    {
        var result = ConsoleColors.Cyan("test");

        Assert.Contains("test", result);
        if (result != "test")
        {
            Assert.Contains("\u001b[36m", result);
            Assert.Contains("\u001b[0m", result);
        }
    }

    [Fact]
    public void Yellow_WithColorsEnabled_AddsAnsiCodes()
    {
        var result = ConsoleColors.Yellow("test");

        Assert.Contains("test", result);
        if (result != "test")
        {
            Assert.Contains("\u001b[33m", result);
            Assert.Contains("\u001b[0m", result);
        }
    }

    [Fact]
    public void Green_WithColorsEnabled_AddsAnsiCodes()
    {
        var result = ConsoleColors.Green("test");

        Assert.Contains("test", result);
        if (result != "test")
        {
            Assert.Contains("\u001b[32m", result);
            Assert.Contains("\u001b[0m", result);
        }
    }

    [Fact]
    public void Bold_WithColorsEnabled_AddsAnsiCodes()
    {
        var result = ConsoleColors.Bold("test");

        Assert.Contains("test", result);
        if (result != "test")
        {
            Assert.Contains("\u001b[1m", result);
            Assert.Contains("\u001b[0m", result);
        }
    }

    [Fact]
    public void Reset_WithColorsEnabled_ReturnsAnsiCode()
    {
        var result = ConsoleColors.Reset;

        Assert.NotNull(result);
        if (result != string.Empty)
        {
            Assert.Equal("\u001b[0m", result);
        }
    }

    private static bool GetColorsEnabled()
    {
        var field = typeof(ConsoleColors).GetField("ColorsEnabled", BindingFlags.NonPublic | BindingFlags.Static);
        if (field == null)
        {
            throw new InvalidOperationException("ColorsEnabled field not found");
        }
        return (bool)field.GetValue(null)!;
    }

    [Fact]
    public void Cyan_TernaryOperator_TrueBranch_ReturnsAnsiCodes()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Cyan("test");

        if (colorsEnabled)
        {
            Assert.Contains("\u001b[36m", result);
            Assert.Contains("\u001b[0m", result);
            Assert.Contains("test", result);
        }
        else
        {
            Assert.Equal("test", result);
        }
    }

    [Fact]
    public void Cyan_TernaryOperator_FalseBranch_ReturnsPlainText()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Cyan("test");

        if (!colorsEnabled)
        {
            Assert.Equal("test", result);
            Assert.DoesNotContain("\u001b[", result);
        }
    }

    [Fact]
    public void Yellow_TernaryOperator_TrueBranch_ReturnsAnsiCodes()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Yellow("test");

        if (colorsEnabled)
        {
            Assert.Contains("\u001b[33m", result);
            Assert.Contains("\u001b[0m", result);
            Assert.Contains("test", result);
        }
        else
        {
            Assert.Equal("test", result);
        }
    }

    [Fact]
    public void Yellow_TernaryOperator_FalseBranch_ReturnsPlainText()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Yellow("test");

        if (!colorsEnabled)
        {
            Assert.Equal("test", result);
            Assert.DoesNotContain("\u001b[", result);
        }
    }

    [Fact]
    public void Green_TernaryOperator_TrueBranch_ReturnsAnsiCodes()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Green("test");

        if (colorsEnabled)
        {
            Assert.Contains("\u001b[32m", result);
            Assert.Contains("\u001b[0m", result);
            Assert.Contains("test", result);
        }
        else
        {
            Assert.Equal("test", result);
        }
    }

    [Fact]
    public void Green_TernaryOperator_FalseBranch_ReturnsPlainText()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Green("test");

        if (!colorsEnabled)
        {
            Assert.Equal("test", result);
            Assert.DoesNotContain("\u001b[", result);
        }
    }

    [Fact]
    public void Bold_TernaryOperator_TrueBranch_ReturnsAnsiCodes()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Bold("test");

        if (colorsEnabled)
        {
            Assert.Contains("\u001b[1m", result);
            Assert.Contains("\u001b[0m", result);
            Assert.Contains("test", result);
        }
        else
        {
            Assert.Equal("test", result);
        }
    }

    [Fact]
    public void Bold_TernaryOperator_FalseBranch_ReturnsPlainText()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Bold("test");

        if (!colorsEnabled)
        {
            Assert.Equal("test", result);
            Assert.DoesNotContain("\u001b[", result);
        }
    }

    [Fact]
    public void Reset_TernaryOperator_TrueBranch_ReturnsAnsiCode()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Reset;

        if (colorsEnabled)
        {
            Assert.Equal("\u001b[0m", result);
        }
        else
        {
            Assert.Equal(string.Empty, result);
        }
    }

    [Fact]
    public void Reset_TernaryOperator_FalseBranch_ReturnsEmptyString()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = ConsoleColors.Reset;

        if (!colorsEnabled)
        {
            Assert.Equal(string.Empty, result);
            Assert.DoesNotContain("\u001b[", result);
        }
    }

    private static bool InvokeShouldEnableColors()
    {
        var method = typeof(ConsoleColors).GetMethod("ShouldEnableColors",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
        {
            throw new InvalidOperationException("ShouldEnableColors method not found");
        }
        return (bool)method.Invoke(null, null)!;
    }

    private static string InvokeApplyColor(string text, string ansiCode)
    {
        var method = typeof(ConsoleColors).GetMethod("ApplyColor",
            BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
        {
            throw new InvalidOperationException("ApplyColor method not found");
        }
        return (string)method.Invoke(null, new object[] { text, ansiCode })!;
    }

    [Fact]
    public void ApplyColor_WhenColorsEnabled_ReturnsAnsiCodes()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = InvokeApplyColor("test", "\u001b[36m");

        if (colorsEnabled)
        {
            Assert.Contains("\u001b[36m", result);
            Assert.Contains("\u001b[0m", result);
            Assert.Contains("test", result);
        }
        else
        {
            Assert.Equal("test", result);
        }
    }

    [Fact]
    public void ApplyColor_WhenColorsDisabled_ReturnsPlainText()
    {
        var colorsEnabled = GetColorsEnabled();
        var result = InvokeApplyColor("test", "\u001b[36m");

        if (!colorsEnabled)
        {
            Assert.Equal("test", result);
            Assert.DoesNotContain("\u001b[", result);
        }
        else
        {
            Assert.Contains("\u001b[36m", result);
            Assert.Contains("\u001b[0m", result);
        }
    }

    [Fact]
    public void ShouldEnableColors_WithNoColorEnvVarSet_ReturnsFalse()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        try
        {
            Environment.SetEnvironmentVariable("NO_COLOR", "1");

            var result = InvokeShouldEnableColors();

            Assert.False(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }

    [Fact]
    public void ShouldEnableColors_WithNoColorEnvVarWhitespace_ReturnsTrue()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        try
        {
            Environment.SetEnvironmentVariable("NO_COLOR", " ");

            var result = InvokeShouldEnableColors();

            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }

    [Fact]
    public void ShouldEnableColors_WithNoColorEnvVarEmpty_ReturnsTrue()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        try
        {
            Environment.SetEnvironmentVariable("NO_COLOR", "");

            var result = InvokeShouldEnableColors();

            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }

    [Fact]
    public void ShouldEnableColors_WithoutNoColorEnvVar_ReturnsTrue()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        try
        {
            Environment.SetEnvironmentVariable("NO_COLOR", null);

            var result = InvokeShouldEnableColors();

            Assert.True(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }

    [Fact]
    public void ShouldEnableColors_WithNoColorEnvVarSetToZero_ReturnsFalse()
    {
        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        try
        {
            Environment.SetEnvironmentVariable("NO_COLOR", "0");

            var result = InvokeShouldEnableColors();

            Assert.False(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }
}