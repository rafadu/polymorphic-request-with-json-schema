namespace Core.Domain;

public class SchemaDefinition
{
    public PayloadType PayloadType { get; }
    public string SchemaJson { get; }
    public int Version { get; }
    public DateTime CreatedAt { get; }

    public SchemaDefinition(PayloadType payloadType, string schemaJson, int version, DateTime createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaJson);
        ArgumentOutOfRangeException.ThrowIfLessThan(version, 1);

        PayloadType = payloadType;
        SchemaJson = schemaJson;
        Version = version;
        CreatedAt = createdAt;
    }
}
