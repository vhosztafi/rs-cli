namespace RoadStatus.Cli;

public sealed class HttpClientFactory
{
    public HttpClient Create()
    {
        return new HttpClient();
    }
}
