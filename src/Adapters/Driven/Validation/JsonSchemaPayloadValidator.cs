using System.Text.Json;
using Core.Domain;
using Core.Ports.Out;
using Json.Schema;

namespace Adapters.Driven.Validation;

public class JsonSchemaPayloadValidator : IPayloadValidator
{
    public IReadOnlyDictionary<string, PropertyAnnotation> Validate(SchemaDefinition schema, JsonElement payload)
    {
        var jsonSchema = JsonSchema.FromText(schema.SchemaJson);
        var result = jsonSchema.Evaluate(payload, new EvaluationOptions { OutputFormat = OutputFormat.List });

        if (!result.IsValid)
        {
            var errors = result.Details
                .Where(d => !d.IsValid && d.Errors != null)
                .GroupBy(d => d.InstanceLocation.ToString().TrimStart('/'))
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<string>)g.SelectMany(d => d.Errors!.Values).ToList());

            throw new PayloadValidationException(errors);
        }

        var annotations = new Dictionary<string, PropertyAnnotation>();
        var propertiesKeyword = jsonSchema.GetKeyword<PropertiesKeyword>();
        if (propertiesKeyword is not null)
        {
            foreach (var (propName, propSchema) in propertiesKeyword.Properties)
            {
                var title = propSchema.GetKeyword<TitleKeyword>()?.Value;
                var description = propSchema.GetKeyword<DescriptionKeyword>()?.Value;
                if (title is not null || description is not null)
                    annotations[propName] = new PropertyAnnotation { Title = title, Description = description };
            }
        }

        return annotations;
    }
}
