using Adapters.Driven.Persistence;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;

namespace Adapters.Tests.Persistence;

public class SchemaDbContextTests : IDisposable
{
    private const string ConnectionString = "Data Source=SchemaDbContextTests;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _connection;

    public SchemaDbContextTests()
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Execute("DROP TABLE IF EXISTS SchemaDefinitions");
        _connection.Dispose();
    }

    private static SchemaDbContext CreateContext(IEnumerable<SchemaSeed>? seeds = null) =>
        new(ConnectionString, seeds ?? []);

    [Fact]
    public async Task InitializeAsync_WhenCalled_CreatesSchemaDefinitionsTable()
    {
        var context = CreateContext();

        await context.InitializeAsync();

        var tableExists = _connection.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='SchemaDefinitions'");
        tableExists.Should().Be(1);
    }

    [Fact]
    public async Task InitializeAsync_WhenTableIsEmpty_InsertsProvidedSeeds()
    {
        var seeds = new[]
        {
            new SchemaSeed("invoice", """{"type":"object"}"""),
            new SchemaSeed("order", """{"type":"object","properties":{}}""")
        };
        var context = CreateContext(seeds);

        await context.InitializeAsync();

        var count = _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM SchemaDefinitions");
        count.Should().Be(2);
    }

    [Fact]
    public async Task InitializeAsync_WhenTableIsEmpty_StoresCorrectPayloadType()
    {
        var seeds = new[] { new SchemaSeed("invoice", """{"type":"object"}""") };
        var context = CreateContext(seeds);

        await context.InitializeAsync();

        var payloadType = _connection.ExecuteScalar<string>(
            "SELECT PayloadType FROM SchemaDefinitions WHERE PayloadType = 'invoice'");
        payloadType.Should().Be("invoice");
    }

    [Fact]
    public async Task InitializeAsync_WhenTableAlreadyHasData_DoesNotInsertSeeds()
    {
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS SchemaDefinitions (
                PayloadType TEXT PRIMARY KEY,
                SchemaJson  TEXT NOT NULL,
                Version     INTEGER NOT NULL,
                CreatedAt   TEXT NOT NULL
            )");
        _connection.Execute(@"
            INSERT INTO SchemaDefinitions (PayloadType, SchemaJson, Version, CreatedAt)
            VALUES ('existing', '{""type"":""object""}', 1, '2026-01-01T00:00:00Z')");

        var seeds = new[] { new SchemaSeed("invoice", """{"type":"object"}""") };
        var context = CreateContext(seeds);

        await context.InitializeAsync();

        var count = _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM SchemaDefinitions");
        count.Should().Be(1);
    }

    [Fact]
    public async Task InitializeAsync_CalledTwice_IsIdempotent()
    {
        var seeds = new[] { new SchemaSeed("invoice", """{"type":"object"}""") };
        var context = CreateContext(seeds);

        await context.InitializeAsync();
        await context.InitializeAsync();

        var count = _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM SchemaDefinitions");
        count.Should().Be(1);
    }
}
