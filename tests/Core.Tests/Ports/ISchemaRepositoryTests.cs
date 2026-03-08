using Core.Domain;
using Core.Ports.Out;
using FluentAssertions;
using NSubstitute;

namespace Core.Tests.Ports;

public class ISchemaRepositoryTests
{
    [Fact]
    public async Task GetByPayloadTypeAsync_WhenSchemaExists_ReturnsSchemaDefinition()
    {
        var payloadType = new PayloadType("invoice");
        var schema = new SchemaDefinition(payloadType, """{"type":"object"}""", version: 1, DateTime.UtcNow);
        var repository = Substitute.For<ISchemaRepository>();
        repository.GetByPayloadTypeAsync(payloadType).Returns(schema);

        var result = await repository.GetByPayloadTypeAsync(payloadType);

        result.Should().Be(schema);
    }

    [Fact]
    public async Task GetByPayloadTypeAsync_WhenSchemaDoesNotExist_ReturnsNull()
    {
        var payloadType = new PayloadType("unknown");
        var repository = Substitute.For<ISchemaRepository>();
        repository.GetByPayloadTypeAsync(payloadType).Returns((SchemaDefinition?)null);

        var result = await repository.GetByPayloadTypeAsync(payloadType);

        result.Should().BeNull();
    }
}
