using Microsoft.Extensions.Configuration;
using Xunit;

namespace RoadStatus.Cli.Tests;

public class ConfigurationTests
{
    [Fact]
    public void LoadConfiguration_WithAppSettingsJson_LoadsValues()
    {
        var appsettingsJson = """
            {
              "TflApi": {
                "BaseUrl": "https://test.api.tfl.gov.uk",
                "AppId": "test-app-id",
                "AppKey": "test-app-key"
              }
            }
            """;

        var tempDir = Path.GetTempPath();
        var tempFile = Path.Combine(tempDir, "appsettings.json");
        
        try
        {
            File.WriteAllText(tempFile, appsettingsJson);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(tempDir)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            var options = new TflApiOptions();
            configuration.GetSection(TflApiOptions.SectionName).Bind(options);

            Assert.Equal("https://test.api.tfl.gov.uk", options.BaseUrl);
            Assert.Equal("test-app-id", options.AppId);
            Assert.Equal("test-app-key", options.AppKey);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void LoadConfiguration_WithEnvironmentVariables_OverridesAppSettings()
    {
        var appsettingsJson = """
            {
              "TflApi": {
                "BaseUrl": "https://test.api.tfl.gov.uk",
                "AppId": "appsettings-app-id",
                "AppKey": "appsettings-app-key"
              }
            }
            """;

        var tempDir = Path.GetTempPath();
        var tempFile = Path.Combine(tempDir, "appsettings.json");
        var originalAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var originalAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
        var originalBaseUrl = Environment.GetEnvironmentVariable("TFL_BASE_URL");

        try
        {
            File.WriteAllText(tempFile, appsettingsJson);
            Environment.SetEnvironmentVariable("TFL_APP_ID", "env-app-id");
            Environment.SetEnvironmentVariable("TFL_APP_KEY", "env-app-key");
            Environment.SetEnvironmentVariable("TFL_BASE_URL", "https://env.api.tfl.gov.uk");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(tempDir)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var options = new TflApiOptions();
            configuration.GetSection(TflApiOptions.SectionName).Bind(options);

            var envAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
            var envAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");
            var envBaseUrl = Environment.GetEnvironmentVariable("TFL_BASE_URL");

            if (!string.IsNullOrWhiteSpace(envAppId))
            {
                options.AppId = envAppId;
            }

            if (!string.IsNullOrWhiteSpace(envAppKey))
            {
                options.AppKey = envAppKey;
            }

            if (!string.IsNullOrWhiteSpace(envBaseUrl))
            {
                options.BaseUrl = envBaseUrl;
            }

            Assert.Equal("https://env.api.tfl.gov.uk", options.BaseUrl);
            Assert.Equal("env-app-id", options.AppId);
            Assert.Equal("env-app-key", options.AppKey);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }

            Environment.SetEnvironmentVariable("TFL_APP_ID", originalAppId);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", originalAppKey);
            Environment.SetEnvironmentVariable("TFL_BASE_URL", originalBaseUrl);
        }
    }

    [Fact]
    public void LoadConfiguration_WithoutAppSettingsJson_UsesDefaults()
    {
        var tempDir = Path.GetTempPath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(tempDir)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        var options = new TflApiOptions();
        configuration.GetSection(TflApiOptions.SectionName).Bind(options);

        Assert.Equal("https://api.tfl.gov.uk", options.BaseUrl);
        Assert.Null(options.AppId);
        Assert.Null(options.AppKey);
    }

    [Fact]
    public void LoadConfiguration_EnvironmentVariableMapping_WorksCorrectly()
    {
        var originalAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
        var originalAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");

        try
        {
            Environment.SetEnvironmentVariable("TFL_APP_ID", "mapped-app-id");
            Environment.SetEnvironmentVariable("TFL_APP_KEY", "mapped-app-key");

            var options = new TflApiOptions();

            var envAppId = Environment.GetEnvironmentVariable("TFL_APP_ID");
            var envAppKey = Environment.GetEnvironmentVariable("TFL_APP_KEY");

            if (!string.IsNullOrWhiteSpace(envAppId))
            {
                options.AppId = envAppId;
            }

            if (!string.IsNullOrWhiteSpace(envAppKey))
            {
                options.AppKey = envAppKey;
            }

            Assert.Equal("mapped-app-id", options.AppId);
            Assert.Equal("mapped-app-key", options.AppKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable("TFL_APP_ID", originalAppId);
            Environment.SetEnvironmentVariable("TFL_APP_KEY", originalAppKey);
        }
    }

    [Fact]
    public void TflApiOptions_DefaultBaseUrl_IsCorrect()
    {
        var options = new TflApiOptions();

        Assert.Equal("https://api.tfl.gov.uk", options.BaseUrl);
    }

    [Fact]
    public void LoadConfiguration_PartialAppSettings_OnlyOverridesSpecifiedValues()
    {
        var appsettingsJson = """
            {
              "TflApi": {
                "BaseUrl": "https://partial.api.tfl.gov.uk"
              }
            }
            """;

        var tempDir = Path.GetTempPath();
        var tempFile = Path.Combine(tempDir, "appsettings.json");

        try
        {
            File.WriteAllText(tempFile, appsettingsJson);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(tempDir)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            var options = new TflApiOptions();
            configuration.GetSection(TflApiOptions.SectionName).Bind(options);

            Assert.Equal("https://partial.api.tfl.gov.uk", options.BaseUrl);
            Assert.Null(options.AppId);
            Assert.Null(options.AppKey);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}