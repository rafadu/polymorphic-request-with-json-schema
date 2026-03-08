using System.Text.Json;
using Core.Domain;
using Core.Ports.In;
using Core.Ports.Out;

namespace Core.UseCases;

public class InterpretPayloadUseCase(ISchemaRepository repository, IPayloadValidator validator)
    : IInterpretPayloadUseCase
{
    public async Task<InterpretationResult> InterpretAsync(PayloadType payloadType, JsonElement payload)
    {
        var schema = await repository.GetByPayloadTypeAsync(payloadType)
            ?? throw new SchemaNotFoundException(payloadType);

        var annotations = validator.Validate(schema, payload);

        return new InterpretationResult(payloadType, payload, annotations);
    }
}
