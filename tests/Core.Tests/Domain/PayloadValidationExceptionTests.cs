using Core.Domain;
using FluentAssertions;

namespace Core.Tests.Domain;

public class PayloadValidationExceptionTests
{
    [Fact]
    public void New_WhenCreated_IsException()
    {
        var errors = new Dictionary<string, IReadOnlyList<string>>
        {
            ["amount"] = ["Amount is required"]
        };

        var exception = new PayloadValidationException(errors);

        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void New_WhenCreated_StoresErrors()
    {
        var errors = new Dictionary<string, IReadOnlyList<string>>
        {
            ["amount"] = ["Amount is required", "Amount must be positive"]
        };

        var exception = new PayloadValidationException(errors);

        exception.Errors.Should().ContainKey("amount");
        exception.Errors["amount"].Should().BeEquivalentTo(["Amount is required", "Amount must be positive"]);
    }

    [Fact]
    public void New_WhenCreatedWithMultipleFields_StoresAllFields()
    {
        var errors = new Dictionary<string, IReadOnlyList<string>>
        {
            ["amount"] = ["Amount is required"],
            ["dueDate"] = ["Due date must be in the future"]
        };

        var exception = new PayloadValidationException(errors);

        exception.Errors.Should().HaveCount(2);
        exception.Errors.Should().ContainKey("amount");
        exception.Errors.Should().ContainKey("dueDate");
    }
}
