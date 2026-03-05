# Claims API
A REST API for managing insurance claims and covers, built with ASP.NET Core, MongoDB, and SQL Server.

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Running the API

### Option 1 — Docker Compose (default)
A `docker-compose.yml` is provided to run MongoDB and SQL Server locally with fixed ports. Set `"UseTestContainers": false` in `appsettings.json` and provide connection strings:

```json
"ConnectionStrings": {
  "SqlServer": "Server=localhost,1433;Database=AuditDb;User Id=sa;Password=a123456!;TrustServerCertificate=True;",
  "MongoDb": "mongodb://localhost:27017"
}
```

Start the containers:

```bash
docker-compose up -d
cd Claims
dotnet run
```

### Option 2 — Testcontainers
Set `"UseTestContainers": true` in `appsettings.json`. Docker is required — MongoDB and SQL Server will be spun up automatically on startup with no manual setup needed.

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

## Architecture
The solution is structured in layers:

```
Claims/
├── Controllers/       # HTTP layer — routes requests, returns responses
├── Services/          # Business logic — validation, orchestration
├── Infrastructure/    # Data access — MongoDB (ClaimsContext) and SQL Server (AuditContext)
├── Domain/            # Core domain models — Claim, Cover
├── Contracts/         # Request and response DTOs
└── Core/              # Shared utilities — Result<T>, ResultType
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
