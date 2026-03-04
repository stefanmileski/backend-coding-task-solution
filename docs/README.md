
# Claims API

A REST API for managing insurance claims and covers, built with ASP.NET Core, MongoDB, and SQL Server.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

Docker is required as the API uses [Testcontainers](https://dotnet.testcontainers.org/) to spin up MongoDB and SQL Server automatically on startup — no manual database setup needed.

## Running the API

```bash
cd Claims
dotnet run
```


The API will be available at `https://localhost:7052`. Swagger UI is available at `https://localhost:7052/swagger`.

## Running the Tests

```bash
cd Claims.Tests
dotnet test
```

Tests include both unit tests (services) and integration tests (controllers via `WebApplicationFactory`). Integration tests spin up their own isolated Testcontainers instances.

## Architecture

The solution is structured in layers:

```
Claims/
├── Controllers/       # HTTP layer — routes requests, returns responses
├── Services/          # Business logic — validation, orchestration
├── Infrastructure/    # Data access — MongoDB (ClaimsContext) and SQL Server (AuditContext)
├── Domain/            # Core domain models — Claim, Cover
├── Contracts/         # Request and response DTOs
└── Common/            # Shared utilities — Result<T>, ResultType
```

### Request Flow

```
HTTP Request
    │
    ▼
Controller        — maps HTTP → service call, returns IActionResult via OkOrError()
    │
    ▼
Service           — validates business rules, returns Result<T>
    │
    ▼
ClaimsContext     — reads/writes to MongoDB via EF Core
```

### Result Pattern

Services return `Result<T>` instead of nullable types. `ResultType` carries the outcome (`Ok`, `NotFound`, `Invalid`, `InternalError`), which the base controller maps to the appropriate HTTP status code and `ProblemDetails` response body.

### Auditing

Every `POST` and `DELETE` request is audited to SQL Server (`ClaimAudits` / `CoverAudits` tables). To avoid blocking the HTTP request, auditing is handled asynchronously:

1. The service writes an audit message to an in-memory `Channel<T>` (non-blocking)
2. The HTTP request returns immediately
3. A `BackgroundService` (`AuditWorker`) drains the channel and persists records to SQL Server

This means audit writes are best-effort — records may be lost if the process crashes before the worker processes them.
