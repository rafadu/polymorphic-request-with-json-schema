using Core.Domain;
using Core.Ports.Out;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Adapters.Driven.Persistence;

public class SqliteSchemaRepository(string connectionString) : ISchemaRepository
{
    public async Task<SchemaDefinition?> GetByPayloadTypeAsync(PayloadType payloadType)
    {
        await using var connection = new SqliteConnection(connectionString);

        var row = await connection.QuerySingleOrDefaultAsync<SchemaDefinitionRow>(
            "SELECT PayloadType, SchemaJson, Version, CreatedAt FROM SchemaDefinitions WHERE PayloadType = @PayloadType",
            new { PayloadType = payloadType.Value });

        return row is null ? null : new SchemaDefinition(
            new PayloadType(row.PayloadType),
            row.SchemaJson,
            row.Version,
            DateTime.Parse(row.CreatedAt));
    }

    private class SchemaDefinitionRow
    {
        public required string PayloadType { get; set; }
        public required string SchemaJson { get; set; }
        public int Version { get; set; }
        public required string CreatedAt { get; set; }
    }
}
