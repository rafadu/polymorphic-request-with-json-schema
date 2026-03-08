using Core.Domain;
using FluentAssertions;

namespace Core.Tests.Domain;

public class SchemaNotFoundExceptionTests
{
    [Fact]
    public void New_WhenCreated_IsException()
    {
        var payloadType = new PayloadType("invoice");

        var exception = new SchemaNotFoundException(payloadType);

        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void New_WhenCreated_StoresPayloadType()
    {
        var payloadType = new PayloadType("invoice");

        var exception = new SchemaNotFoundException(payloadType);

        exception.PayloadType.Should().Be(payloadType);
    }

    [Fact]
    public void New_WhenCreated_MessageContainsPayloadTypeValue()
    {
        var payloadType = new PayloadType("invoice");

        var exception = new SchemaNotFoundException(payloadType);

        exception.Message.Should().Contain("invoice");
    }
}
