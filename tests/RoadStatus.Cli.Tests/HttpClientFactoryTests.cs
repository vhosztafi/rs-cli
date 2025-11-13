using Xunit;

namespace RoadStatus.Cli.Tests;

public class HttpClientFactoryTests
{
    [Fact]
    public void Create_ReturnsNewHttpClient()
    {
        var factory = new HttpClientFactory();

        var httpClient = factory.Create();

        Assert.NotNull(httpClient);
    }

    [Fact]
    public void Create_ReturnsDifferentInstances()
    {
        var factory = new HttpClientFactory();

        var client1 = factory.Create();
        var client2 = factory.Create();

        Assert.NotSame(client1, client2);
    }

    [Fact]
    public void Create_ReturnsHttpClientInstance()
    {
        var factory = new HttpClientFactory();

        var httpClient = factory.Create();

        Assert.IsType<HttpClient>(httpClient);
    }
}

