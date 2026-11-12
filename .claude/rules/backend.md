---
paths:
  - "TodoApp.*/**"
  - "*.sln"
---

# Project Rules & Assistant Guidelines (.NET Backend)

Rules for the TodoApp backend. Loaded when Claude works with `TodoApp.*` files; shared communication and workflow rules live in the root `CLAUDE.md`. Tasks: GitHub Issues with the `backend` label.

## 1. Context

* **Project:** TodoApp is a RESTful API for managing tasks (creation, categorization, completion statuses) with the `todo-app-ui` client.
* **Role:** Senior .NET/C# Developer.

## 2. Tech Stack

* **Platform:** .NET 8+ (C# 12+), ASP.NET Core Web API.
* **ORM & Data:** Entity Framework Core, SQL Server (`AppDbContext`).
* **Documentation:** OpenAPI / Swagger.
* **Language constructs:** `record` for DTOs, extension methods for mapping.

## 3. Architecture

Classic N-Tier architecture with elements of Clean Architecture and CQRS:

* `TodoApp.API` (Presentation Layer): thin controllers. Accept HTTP requests, delegate to services, return the correct status codes (`200 OK`, `201 Created`, `204 NoContent`, `404 NotFound`). No business logic.
* `TodoApp.Services` (Business Logic Layer): services (for example, `CategoryService`). Apply business rules, work with `AppDbContext`, throw domain exceptions. CQRS style: `Create` returns a DTO; `Update` and `Delete` only change state and return `Task`.
* `TodoApp.Core` (Domain & Contracts Layer): the independent core. Contains Entities (`Category`, `TodoTask`), DTOs (`record`), interfaces (`ICategoryService`), custom exceptions (`NotFoundException`) and mapping methods.
* `TodoApp.Data` (Data Access Layer): `AppDbContext` configuration, database connection and migrations.
* `Mappings` (Data Conversion): static classes and extension methods (`ToEntity`, `ToCategoryDto`). Strictly in-memory, with no external dependencies.

## 4. Code Style

* **Asynchrony:** all I/O operations are asynchronous (`async`/`await`). Method names have the `Async` suffix. Pass `CancellationToken` through.
* **Error handling:** services do not return `null` or `bool` when an entity is missing; they throw custom exceptions (`throw new NotFoundException(...)`). Controllers either catch them or rely on a global Exception Filter/Middleware that returns a standardized HTTP status.
* **DTOs:** defined only as immutable `record`s with the `Dto` suffix (for example, `CategoryCreateDto`, `CategoryDto`).
* **Input validation:** request rules (`[Required]`, `[MaxLength]`, `[EmailAddress]`, etc.) are set with `System.ComponentModel.DataAnnotations` attributes on the parameters of input DTOs (`*CreateDto`, `*UpdateDto`, `RegisterDto`, `LoginDto`); `[ApiController]` returns `400` on its own. Output DTOs carry no attributes.
* **DB schema constraints:** length, `NOT NULL` and indexes are defined only via Fluent API in `AppDbContext`. Entities (`Entities`) stay free of validation attributes.
* **Settings (Options pattern):** configuration needed by services is described by a class with the `Settings` suffix in the `Options` folder of the consuming project (`TodoApp.Services/Options`, `namespace TodoApp.Services.Options`). Rules:
  * A plain class (not a `record`), properties `{ get; init; }`, the section name in `const string SectionName`, validity rules via `System.ComponentModel.DataAnnotations`.
  * Registration: `AddOptions<T>().BindConfiguration(T.SectionName).ValidateDataAnnotations().ValidateOnStart()`, so broken configuration stops startup instead of failing on the first request.
  * Services receive `IOptions<T>` (not `IConfiguration` and not `IOptionsMonitor<T>`, since hot reload is not needed). The value is read in the constructor: `_settings = options.Value`.
  * Secrets (signing keys, passwords) are not kept in `appsettings.json`: only user-secrets locally and environment variables on the server.
* **Mapping:** PascalCase for extension methods (`ToEntity`, `MapTo`).
* **Working with EF Core:**
  * Reads (`GetAll`, `GetById`): via projections `.Select(x => new Dto(...)).FirstOrDefaultAsync()` or `.AsNoTracking()`, to avoid loading tracked entities unnecessarily (over-fetching).
  * Changes (`Update`, `Delete`): the entity is fetched with a tracked query filtered by owner (`_context.DbSet.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userId)`) to activate Change Tracking and isolate user data (another user's record is not found, so the result is `404`), modified in memory through a mapping method and saved with `SaveChangesAsync()`.

* **Tests (`TodoApp.Tests`):** helper code (`TodoAppFactory`, extension methods for `HttpClient`) lives in a separate `Helpers/` folder with `namespace TodoApp.Tests.Helpers`, included via `<Using Include>` in the `.csproj`. One helper class covers one area (`HttpClientAuthExtensions`, `HttpClientDataExtensions`), with no shared "junk drawer". The tests themselves sit in the project root and are grouped into folders by resource (`Auth/`, `Tasks/`) only when there are many of them.
  * Integration tests use a real LocalDB database `TodoAppDb_Tests` with the same migrations as production (not EF InMemory or SQLite: they do not verify FKs, cascades and transactions). The JWT secret in tests is supplied via `AddInMemoryCollection` in the factory.
  * Each test creates its own users with a unique email (`Guid`) and does not depend on execution order or database state.
  * After another user's attempt to change or delete a record, a data-isolation test reads it as the owner and verifies the data is unchanged, not just that the status is `404`.
  * Test names: `Method_Condition_Result`.

## 5. What we do not do

* Do not return database entities (`Entities`) directly from controllers. Always DTOs.
* Do not place business logic in controllers or access `AppDbContext` from them.
* Do not call `AppDbContext` or interact with the database inside mapping classes (`Mappings/*.cs`).
* Do not return data (DTOs or Entities) from `Update` and `Delete` service methods. They return `Task`, and at the API level respond with `204 NoContent`.
* Do not use `_context.Update()` for partial updates.
* Do not use `FindAsync` for GET operations (reads) or for entities with an owner (`UserId`): it searches by key only and bypasses the user filter.
* Do not use blocking synchronous calls (`.Result`, `.Wait()`).
