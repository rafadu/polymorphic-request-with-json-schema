using Core.Domain;
using FluentAssertions;

namespace Core.Tests.Domain;

public class PayloadTypeTests
{
    [Fact]
    public void New_WhenValueIsValid_StoresLowercased()
    {
        var payloadType = new PayloadType("Invoice");

        payloadType.Value.Should().Be("invoice");
    }

    [Fact]
    public void New_WhenValueIsAlreadyLowercase_StoresAsIs()
    {
        var payloadType = new PayloadType("invoice");

        payloadType.Value.Should().Be("invoice");
    }

    [Fact]
    public void New_WhenValueIsEmpty_ThrowsArgumentException()
    {
        var act = () => new PayloadType("");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void New_WhenValueIsWhitespace_ThrowsArgumentException()
    {
        var act = () => new PayloadType("   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equality_WhenSameValue_AreEqual()
    {
        var a = new PayloadType("invoice");
        var b = new PayloadType("INVOICE");

        a.Should().Be(b);
    }

    [Fact]
    public void Equality_WhenDifferentValues_AreNotEqual()
    {
        var a = new PayloadType("invoice");
        var b = new PayloadType("order");

        a.Should().NotBe(b);
    }
}
