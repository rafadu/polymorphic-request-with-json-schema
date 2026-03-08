namespace Core.Domain;

public record PayloadType
{
    public string Value { get; }

    public PayloadType(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.ToLowerInvariant();
    }
}
