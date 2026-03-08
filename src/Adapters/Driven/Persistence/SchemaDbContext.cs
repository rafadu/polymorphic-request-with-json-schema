using Dapper;
using Microsoft.Data.Sqlite;

namespace Adapters.Driven.Persistence;

public record SchemaSeed(string PayloadType, string SchemaJson);

public class SchemaDbContext(string connectionString, IEnumerable<SchemaSeed> seeds)
{
    public async Task InitializeAsync()
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS SchemaDefinitions (
                PayloadType TEXT PRIMARY KEY,
                SchemaJson  TEXT NOT NULL,
                Version     INTEGER NOT NULL,
                CreatedAt   TEXT NOT NULL
            )");

        var count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM SchemaDefinitions");
        if (count > 0)
            return;

        foreach (var seed in seeds)
        {
            await connection.ExecuteAsync(@"
                INSERT INTO SchemaDefinitions (PayloadType, SchemaJson, Version, CreatedAt)
                VALUES (@PayloadType, @SchemaJson, 1, @CreatedAt)",
                new { seed.PayloadType, seed.SchemaJson, CreatedAt = DateTime.UtcNow.ToString("o") });
        }
    }
}
