# Project: Polymorphic Payload Interpreter API

## Goal
HTTP endpoint that receives a `payloadType` route parameter and a JSON body, retrieves the matching JSON Schema from SQLite, validates and interprets the payload, and returns a structured result.

## Stack
- .NET 10 / C# — Minimal API
- `System.Text.Json` — serialization (no Newtonsoft)
- `JsonSchema.Net` (json-everything) — JSON Schema validation
- `Microsoft.Data.Sqlite` + `Dapper` — SQLite access
- `xUnit` + `FluentAssertions` + `NSubstitute` — tests

---

## Architecture: Hexagonal (Ports & Adapters)

```
/src
  /Core                          ← Domain (no framework dependencies)
    /Domain
      PayloadType.cs             ← value object
      SchemaDefinition.cs        ← entity
      InterpretationResult.cs    ← value object
    /Ports
      /In
        IInterpretPayloadUseCase.cs
      /Out
        ISchemaRepository.cs
    /UseCases
      InterpretPayloadUseCase.cs

  /Adapters
    /Driven                      ← output adapters
      /Persistence
        SqliteSchemaRepository.cs
        SchemaDbContext.cs        ← schema init / migrations
    /Driving                     ← input adapters
      /Http
        PayloadEndpoints.cs
        /Mappers
        /Dto
          InterpretRequestDto.cs
          InterpretResponseDto.cs

  /Infrastructure
    /DependencyInjection
      CoreServiceExtensions.cs
      PersistenceServiceExtensions.cs
    /Configuration
      AppSettings.cs

/tests
  /Core.Tests                    ← unit tests (pure domain + use cases)
  /Adapters.Tests                ← integration tests (SQLite in-memory, HTTP)
```

### Dependency Rule
`Core` has zero references to framework, EF, SQLite, or ASP.NET.  
Adapters depend on Core. Infrastructure wires everything. Never invert this.

---

## Request Contract

```
POST /api/payload/{payloadType}
Content-Type: application/json

{ ...arbitrary payload body... }
```

- `payloadType` is a route parameter (string, case-insensitive)
- Body is raw JSON — deserialize as `JsonElement` initially
- Endpoint must not assume shape of body before schema validation

## Response Contract (RFC 7807 for errors)

**Success — 200**
```json
{
  "payloadType": "invoice",
  "valid": true,
  "data": { ...interpreted fields... }
}
```

**Validation failure — 422**
```json
{
  "type": "/errors/payload-validation",
  "title": "Payload validation failed",
  "status": 422,
  "errors": { "fieldPath": ["message"] }
}
```

**Unknown payloadType — 404**
```json
{
  "type": "/errors/schema-not-found",
  "title": "Schema not found for payload type",
  "status": 404
}
```

---

## Domain Rules

- `PayloadType` is a value object — validated, non-empty, lowercased
- `SchemaDefinition` holds `PayloadType`, raw JSON Schema string, and metadata (version, created at)
- `InterpretationResult` carries validated data and any annotation from schema (`title`, `description` on properties)
- Use case receives `(PayloadType, JsonElement)` → returns `InterpretationResult`
- All domain exceptions are typed: `SchemaNotFoundException`, `PayloadValidationException`

---

## SQLite Schema Storage

Table: `SchemaDefinitions`

| Column        | Type    | Notes                          |
|---------------|---------|--------------------------------|
| PayloadType   | TEXT PK | lowercase, e.g. `invoice`      |
| SchemaJson    | TEXT    | full JSON Schema string        |
| Version       | INTEGER | incremented on update          |
| CreatedAt     | TEXT    | ISO 8601                       |

- Schema is seeded at startup from `/Infrastructure/Seeds/*.schema.json`
- Seed only if table is empty — never overwrite existing rows
- Use `Microsoft.Data.Sqlite` with Dapper; no EF Core

---

## TDD Rules

**Red → Green → Refactor — always.**

- Write the failing test first. Never write production code without a red test.
- Unit tests live in `Core.Tests` and must not touch SQLite, HTTP, or file system
- Use `NSubstitute` to mock `ISchemaRepository` in use case tests
- Integration tests in `Adapters.Tests` use SQLite in-memory (`DataSource=:memory:`)
- Test naming: `MethodOrScenario_Context_ExpectedBehavior`
  - e.g. `Interpret_WhenSchemaNotFound_ThrowsSchemaNotFoundException`
- One logical assertion per test (multiple `.Should()` on same object is fine)
- Test coverage targets: Core 100%, Adapters ≥ 80%

---

## .NET / C# Conventions

- File-scoped namespaces (`namespace Project.Core.Domain;`)
- Records for value objects and DTOs; classes for entities and services
- `required` keyword for mandatory properties instead of constructor enforcement where appropriate
- Nullable reference types enabled (`<Nullable>enable</Nullable>`) — no `#nullable disable`
- Async all the way: every I/O method is `async Task<T>`, no `.Result` or `.Wait()`
- Primary constructors for services (C# 12)
- `IResult` returns from all Minimal API handlers
- `ProblemDetails` via `TypedResults.Problem(...)` — never raw string error responses
- Internal classes by default; make `public` only what crosses layer boundaries

---

## Commands
```bash
dotnet build                          # must have zero warnings
dotnet test                           # all tests green before any commit
dotnet test --filter "Category=Unit"  # unit tests only
dotnet format                         # run before committing
```

---

## Verification Checklist (run after each feature)
1. `dotnet build` — zero warnings
2. `dotnet test` — all green
3. Test manually with three cases:
   - Valid payload for existing `payloadType`
   - Invalid payload (fails schema) for existing `payloadType`
   - Request with unknown `payloadType`

---

## What NOT to Do
- Never reference `Microsoft.Data.Sqlite`, `Dapper`, or `JsonSchema.Net` from `Core`
- Never use `dynamic` or untyped `object` for payload data
- Never swallow `JsonException` — convert to `PayloadValidationException` at the adapter boundary
- Never load schema files from disk at request time — seed at startup, read from DB
- Never add NuGet packages without proposing and confirming first
- Never write production code before a failing test exists

---

## Context Compaction Rules
When compacting, always preserve:
- Implemented `payloadType` values and their schema status (seeded / tested)
- Any change to the request/response contract
- Ports that have been implemented vs. pending
- Failing tests that are intentionally left red (pending implementation)