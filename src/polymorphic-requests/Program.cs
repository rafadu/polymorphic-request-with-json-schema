using Adapters.Driven.Persistence;
using Adapters.Driving.Http;
using Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=polymorphic.db";

var seedsPath = Path.Combine(builder.Environment.ContentRootPath, "Infrastructure", "Seeds");
var seeds = Directory.GetFiles(seedsPath, "*.schema.json")
    .Select(f => new SchemaSeed(
        Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f)),
        File.ReadAllText(f)));

builder.Services
    .AddCoreServices()
    .AddPersistence(connectionString, seeds);

var app = builder.Build();

var dbContext = app.Services.GetRequiredService<SchemaDbContext>();
await dbContext.InitializeAsync();

app.MapPayloadEndpoints();

app.Run();

public partial class Program { }
