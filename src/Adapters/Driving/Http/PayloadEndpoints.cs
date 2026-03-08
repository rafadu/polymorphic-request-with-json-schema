using System.Text.Json;
using Core.Domain;
using Core.Ports.In;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Adapters.Driving.Http;

public static class PayloadEndpoints
{
    private const string SchemaNotFoundType = "/errors/schema-not-found";
    private const string PayloadValidationType = "/errors/payload-validation";

    public static IEndpointRouteBuilder MapPayloadEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payload/{payloadType}", HandleAsync);
        return app;
    }

    private static async Task<IResult> HandleAsync(
        string payloadType,
        JsonElement body,
        IInterpretPayloadUseCase useCase)
    {
        try
        {
            var type = new PayloadType(payloadType);
            var result = await useCase.InterpretAsync(type, body);
            return TypedResults.Ok(new
            {
                payloadType = result.PayloadType.Value,
                valid = true,
                data = result.Data
            });
        }
        catch (SchemaNotFoundException)
        {
            return TypedResults.Problem(
                title: "Schema not found for payload type",
                type: SchemaNotFoundType,
                statusCode: 404);
        }
        catch (PayloadValidationException ex)
        {
            return TypedResults.Problem(
                title: "Payload validation failed",
                type: PayloadValidationType,
                statusCode: 422,
                extensions: new Dictionary<string, object?> { ["errors"] = ex.Errors });
        }
    }
}
