using System.Text.Json;
using Core.Domain;

namespace Core.Ports.In;

public interface IInterpretPayloadUseCase
{
    Task<InterpretationResult> InterpretAsync(PayloadType payloadType, JsonElement payload);
}
