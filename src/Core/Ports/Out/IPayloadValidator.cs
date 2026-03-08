using System.Text.Json;
using Core.Domain;

namespace Core.Ports.Out;

public interface IPayloadValidator
{
    IReadOnlyDictionary<string, PropertyAnnotation> Validate(SchemaDefinition schema, JsonElement payload);
}
