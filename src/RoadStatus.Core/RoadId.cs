namespace RoadStatus.Core;

public readonly record struct RoadId
{
    private readonly string _value;

    private RoadId(string value)
    {
        _value = value;
    }

    public static RoadId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Road ID cannot be null or empty.", nameof(value));
        }

        return new RoadId(value);
    }

    public override string ToString() => _value;
}

