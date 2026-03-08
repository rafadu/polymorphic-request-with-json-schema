using Core.Domain;
using FluentAssertions;

namespace Core.Tests.Domain;

public class SchemaDefinitionTests
{
    private static readonly PayloadType ValidPayloadType = new("invoice");
    private const string ValidSchemaJson = """{"type":"object"}""";

    [Fact]
    public void New_WhenAllPropertiesAreValid_StoresProperties()
    {
        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var schema = new SchemaDefinition(ValidPayloadType, ValidSchemaJson, version: 1, createdAt);

        schema.PayloadType.Should().Be(ValidPayloadType);
        schema.SchemaJson.Should().Be(ValidSchemaJson);
        schema.Version.Should().Be(1);
        schema.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void New_WhenSchemaJsonIsEmpty_ThrowsArgumentException()
    {
        var act = () => new SchemaDefinition(ValidPayloadType, schemaJson: "", version: 1, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_WhenSchemaJsonIsWhitespace_ThrowsArgumentException()
    {
        var act = () => new SchemaDefinition(ValidPayloadType, schemaJson: "   ", version: 1, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_WhenVersionIsZero_ThrowsArgumentOutOfRangeException()
    {
        var act = () => new SchemaDefinition(ValidPayloadType, ValidSchemaJson, version: 0, DateTime.UtcNow);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void New_WhenVersionIsNegative_ThrowsArgumentOutOfRangeException()
    {
        var act = () => new SchemaDefinition(ValidPayloadType, ValidSchemaJson, version: -1, DateTime.UtcNow);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
