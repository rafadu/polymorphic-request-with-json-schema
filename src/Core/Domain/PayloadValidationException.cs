namespace Core.Domain;

public class PayloadValidationException(IReadOnlyDictionary<string, IReadOnlyList<string>> errors)
    : Exception("Payload validation failed")
{
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Errors { get; } = errors;
}
