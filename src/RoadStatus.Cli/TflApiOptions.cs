namespace RoadStatus.Cli;

public class TflApiOptions
{
    public const string SectionName = "TflApi";

    public string BaseUrl { get; set; } = "https://api.tfl.gov.uk";
    public string? AppId { get; set; }
    public string? AppKey { get; set; }
}

