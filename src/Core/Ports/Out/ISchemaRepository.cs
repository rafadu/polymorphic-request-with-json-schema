using Core.Domain;

namespace Core.Ports.Out;

public interface ISchemaRepository
{
    Task<SchemaDefinition?> GetByPayloadTypeAsync(PayloadType payloadType);
}
