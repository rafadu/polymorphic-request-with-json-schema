using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Core.Domain;
using Core.Ports.In;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Adapters.Tests.Http;

public class PayloadEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PayloadEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClientWith(IInterpretPayloadUseCase useCase) =>
        _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.AddSingleton(useCase)))
        .CreateClient();

    [Fact]
    public async Task Post_WhenPayloadIsValid_Returns200WithPayloadTypeAndValidTrue()
    {
        var payloadType = new PayloadType("invoice");
        var data = JsonDocument.Parse("""{"amount":100}""").RootElement;
        var result = new InterpretationResult(payloadType, data, new Dictionary<string, PropertyAnnotation>());
        var useCase = Substitute.For<IInterpretPayloadUseCase>();
        useCase.InterpretAsync(Arg.Any<PayloadType>(), Arg.Any<JsonElement>()).Returns(result);

        var response = await CreateClientWith(useCase)
            .PostAsJsonAsync("/api/payload/invoice", new { amount = 100 });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("payloadType").GetString().Should().Be("invoice");
        body.GetProperty("valid").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task Post_WhenPayloadIsValid_Returns200WithData()
    {
        var payloadType = new PayloadType("invoice");
        var data = JsonDocument.Parse("""{"amount":100}""").RootElement;
        var result = new InterpretationResult(payloadType, data, new Dictionary<string, PropertyAnnotation>());
        var useCase = Substitute.For<IInterpretPayloadUseCase>();
        useCase.InterpretAsync(Arg.Any<PayloadType>(), Arg.Any<JsonElement>()).Returns(result);

        var response = await CreateClientWith(useCase)
            .PostAsJsonAsync("/api/payload/invoice", new { amount = 100 });

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("data").GetProperty("amount").GetInt32().Should().Be(100);
    }

    [Fact]
    public async Task Post_WhenSchemaNotFound_Returns404WithProblemDetails()
    {
        var useCase = Substitute.For<IInterpretPayloadUseCase>();
        useCase.InterpretAsync(Arg.Any<PayloadType>(), Arg.Any<JsonElement>())
            .ThrowsAsync(new SchemaNotFoundException(new PayloadType("unknown")));

        var response = await CreateClientWith(useCase)
            .PostAsJsonAsync("/api/payload/unknown", new { });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("status").GetInt32().Should().Be(404);
        body.GetProperty("type").GetString().Should().Be("/errors/schema-not-found");
    }

    [Fact]
    public async Task Post_WhenPayloadIsInvalid_Returns422WithProblemDetails()
    {
        var errors = new Dictionary<string, IReadOnlyList<string>>
        {
            ["amount"] = ["Amount is required"]
        };
        var useCase = Substitute.For<IInterpretPayloadUseCase>();
        useCase.InterpretAsync(Arg.Any<PayloadType>(), Arg.Any<JsonElement>())
            .ThrowsAsync(new PayloadValidationException(errors));

        var response = await CreateClientWith(useCase)
            .PostAsJsonAsync("/api/payload/invoice", new { });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("status").GetInt32().Should().Be(422);
        body.GetProperty("type").GetString().Should().Be("/errors/payload-validation");
    }

    [Fact]
    public async Task Post_WhenPayloadIsInvalid_Returns422WithErrors()
    {
        var errors = new Dictionary<string, IReadOnlyList<string>>
        {
            ["amount"] = ["Amount is required"]
        };
        var useCase = Substitute.For<IInterpretPayloadUseCase>();
        useCase.InterpretAsync(Arg.Any<PayloadType>(), Arg.Any<JsonElement>())
            .ThrowsAsync(new PayloadValidationException(errors));

        var response = await CreateClientWith(useCase)
            .PostAsJsonAsync("/api/payload/invoice", new { });

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("errors").GetProperty("amount")[0].GetString().Should().Be("Amount is required");
    }
}
