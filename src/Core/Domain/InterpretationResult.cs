using System.Text.Json;

namespace Core.Domain;

public record InterpretationResult(
    PayloadType PayloadType,
    JsonElement Data,
    IReadOnlyDictionary<string, PropertyAnnotation> Annotations);

public record PropertyAnnotation
{
    public string? Title { get; init; }
    public string? Description { get; init; }
}
