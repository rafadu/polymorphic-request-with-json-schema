using System.Text.Json;
using Core.Domain;
using FluentAssertions;

namespace Core.Tests.Domain;

public class InterpretationResultTests
{
    private static readonly PayloadType ValidPayloadType = new("invoice");

    private static JsonElement ParseJson(string json) =>
        JsonDocument.Parse(json).RootElement;

    [Fact]
    public void New_WhenAllPropertiesAreProvided_StoresPayloadType()
    {
        var data = ParseJson("""{"amount":100}""");
        var annotations = new Dictionary<string, PropertyAnnotation>
        {
            ["amount"] = new PropertyAnnotation { Title = "Amount", Description = "Invoice amount" }
        };

        var result = new InterpretationResult(ValidPayloadType, data, annotations);

        result.PayloadType.Should().Be(ValidPayloadType);
    }

    [Fact]
    public void New_WhenAllPropertiesAreProvided_StoresData()
    {
        var data = ParseJson("""{"amount":100}""");
        var annotations = new Dictionary<string, PropertyAnnotation>();

        var result = new InterpretationResult(ValidPayloadType, data, annotations);

        result.Data.GetRawText().Should().Be("""{"amount":100}""");
    }

    [Fact]
    public void New_WhenAnnotationsAreProvided_StoresAnnotations()
    {
        var data = ParseJson("""{"amount":100}""");
        var annotation = new PropertyAnnotation { Title = "Amount", Description = "Invoice amount" };
        var annotations = new Dictionary<string, PropertyAnnotation> { ["amount"] = annotation };

        var result = new InterpretationResult(ValidPayloadType, data, annotations);

        result.Annotations.Should().ContainKey("amount");
        result.Annotations["amount"].Title.Should().Be("Amount");
        result.Annotations["amount"].Description.Should().Be("Invoice amount");
    }

    [Fact]
    public void New_WhenAnnotationsIsEmpty_IsValid()
    {
        var data = ParseJson("""{"amount":100}""");
        var annotations = new Dictionary<string, PropertyAnnotation>();

        var result = new InterpretationResult(ValidPayloadType, data, annotations);

        result.Annotations.Should().BeEmpty();
    }

    [Fact]
    public void PropertyAnnotation_WhenTitleAndDescriptionAreNull_IsValid()
    {
        var annotation = new PropertyAnnotation();

        annotation.Title.Should().BeNull();
        annotation.Description.Should().BeNull();
    }

    [Fact]
    public void PropertyAnnotation_WhenOnlyTitleIsSet_StoresTitle()
    {
        var annotation = new PropertyAnnotation { Title = "Amount" };

        annotation.Title.Should().Be("Amount");
        annotation.Description.Should().BeNull();
    }
}
