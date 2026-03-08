using Adapters.Driven.Persistence;
using Core.Domain;
using Core.Ports.Out;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;

namespace Adapters.Tests.Persistence;

public class SqliteSchemaRepositoryTests : IDisposable
{
    private const string ConnectionString = "Data Source=SchemaRepoTests;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _connection;

    public SqliteSchemaRepositoryTests()
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS SchemaDefinitions (
                PayloadType TEXT PRIMARY KEY,
                SchemaJson  TEXT NOT NULL,
                Version     INTEGER NOT NULL,
                CreatedAt   TEXT NOT NULL
            )");
    }

    public void Dispose()
    {
        _connection.Execute("DROP TABLE IF EXISTS SchemaDefinitions");
        _connection.Dispose();
    }

    private SqliteSchemaRepository CreateRepository() => new(ConnectionString);

    [Fact]
    public async Task GetByPayloadTypeAsync_WhenSchemaExists_ReturnsSchemaDefinition()
    {
        _connection.Execute(@"
            INSERT INTO SchemaDefinitions (PayloadType, SchemaJson, Version, CreatedAt)
            VALUES ('invoice', '{""type"":""object""}', 1, '2026-01-01T00:00:00Z')");
        var repository = CreateRepository();

        var result = await repository.GetByPayloadTypeAsync(new PayloadType("invoice"));

        result.Should().NotBeNull();
        result!.PayloadType.Value.Should().Be("invoice");
        result.SchemaJson.Should().Be("""{"type":"object"}""");
        result.Version.Should().Be(1);
    }

    [Fact]
    public async Task GetByPayloadTypeAsync_WhenSchemaDoesNotExist_ReturnsNull()
    {
        var repository = CreateRepository();

        var result = await repository.GetByPayloadTypeAsync(new PayloadType("unknown"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByPayloadTypeAsync_WhenPayloadTypeIsUppercase_ReturnsSchema()
    {
        _connection.Execute(@"
            INSERT INTO SchemaDefinitions (PayloadType, SchemaJson, Version, CreatedAt)
            VALUES ('invoice', '{""type"":""object""}', 1, '2026-01-01T00:00:00Z')");
        var repository = CreateRepository();

        var result = await repository.GetByPayloadTypeAsync(new PayloadType("INVOICE"));

        result.Should().NotBeNull();
        result!.PayloadType.Value.Should().Be("invoice");
    }

    [Fact]
    public async Task GetByPayloadTypeAsync_ImplementsISchemaRepository()
    {
        var repository = CreateRepository();

        repository.Should().BeAssignableTo<ISchemaRepository>();
    }
}
