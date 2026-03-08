namespace Core.Domain;

public class SchemaNotFoundException(PayloadType payloadType)
    : Exception($"Schema not found for payload type '{payloadType.Value}'")
{
    public PayloadType PayloadType { get; } = payloadType;
}
