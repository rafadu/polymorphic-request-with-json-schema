using Adapters.Driven.Persistence;
using Adapters.Driven.Validation;
using Core.Ports.Out;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString,
        IEnumerable<SchemaSeed> seeds)
    {
        services.AddSingleton(new SchemaDbContext(connectionString, seeds));
        services.AddScoped<ISchemaRepository>(_ => new SqliteSchemaRepository(connectionString));
        services.AddScoped<IPayloadValidator, JsonSchemaPayloadValidator>();
        return services;
    }
}
