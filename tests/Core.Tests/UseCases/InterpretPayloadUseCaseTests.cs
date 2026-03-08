using System.Text.Json;
using Core.Domain;
using Core.Ports.Out;
using Core.UseCases;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Core.Tests.UseCases;

public class InterpretPayloadUseCaseTests
{
    private readonly ISchemaRepository _repository = Substitute.For<ISchemaRepository>();
    private readonly IPayloadValidator _validator = Substitute.For<IPayloadValidator>();
    private readonly InterpretPayloadUseCase _useCase;

    public InterpretPayloadUseCaseTests()
    {
        _useCase = new InterpretPayloadUseCase(_repository, _validator);
    }

    private static JsonElement ParseJson(string json) =>
        JsonDocument.Parse(json).RootElement;

    [Fact]
    public async Task Interpret_WhenSchemaNotFound_ThrowsSchemaNotFoundException()
    {
        var payloadType = new PayloadType("invoice");
        var payload = ParseJson("""{"amount":100}""");
        _repository.GetByPayloadTypeAsync(payloadType).Returns((SchemaDefinition?)null);

        var act = () => _useCase.InterpretAsync(payloadType, payload);

        await act.Should().ThrowAsync<SchemaNotFoundException>()
            .Where(e => e.PayloadType == payloadType);
    }

    [Fact]
    public async Task Interpret_WhenSchemaFoundAndPayloadValid_ReturnsInterpretationResult()
    {
        var payloadType = new PayloadType("invoice");
        var payload = ParseJson("""{"amount":100}""");
        var schema = new SchemaDefinition(payloadType, """{"type":"object"}""", version: 1, DateTime.UtcNow);
        var annotations = new Dictionary<string, PropertyAnnotation>
        {
            ["amount"] = new PropertyAnnotation { Title = "Amount" }
        };
        _repository.GetByPayloadTypeAsync(payloadType).Returns(schema);
        _validator.Validate(schema, payload).Returns(annotations);

        var result = await _useCase.InterpretAsync(payloadType, payload);

        result.PayloadType.Should().Be(payloadType);
        result.Data.GetRawText().Should().Be(payload.GetRawText());
        result.Annotations.Should().BeEquivalentTo(annotations);
    }

    [Fact]
    public async Task Interpret_WhenSchemaFoundAndPayloadInvalid_ThrowsPayloadValidationException()
    {
        var payloadType = new PayloadType("invoice");
        var payload = ParseJson("""{}""");
        var schema = new SchemaDefinition(payloadType, """{"type":"object"}""", version: 1, DateTime.UtcNow);
        var validationErrors = new Dictionary<string, IReadOnlyList<string>>
        {
            ["amount"] = ["Amount is required"]
        };
        _repository.GetByPayloadTypeAsync(payloadType).Returns(schema);
        _validator.Validate(schema, payload).Throws(new PayloadValidationException(validationErrors));

        var act = () => _useCase.InterpretAsync(payloadType, payload);

        await act.Should().ThrowAsync<PayloadValidationException>()
            .Where(e => e.Errors.ContainsKey("amount"));
    }
}
