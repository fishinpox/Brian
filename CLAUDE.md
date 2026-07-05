# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Purpose

Brian is a VTuber/creator ecosystem backend built on Clean Architecture + microservices (ASP.NET Core .NET 9). The product covers three apps: Fan App, Creator App, and Agency/CRM App. Feature specs live in `Documentation/`.

## Solution Location

```
Solutions/SampleSolution/CleanArchitectureSample/
```

All `dotnet` commands run from that directory.

## Build & Run

```bash
cd Solutions/SampleSolution/CleanArchitectureSample

dotnet build          # Build all 36 projects
dotnet restore        # Restore NuGet packages

# Run individual services (each on its own port):
dotnet run --project src/Gateway/Gateway.csproj                                      # :7000
dotnet run --project src/Services/Identity/Identity.API/Identity.API.csproj         # :7001
dotnet run --project src/Services/Calendar/Calendar.API/Calendar.API.csproj         # :7002

# Start infrastructure (SQL Server, Redis, RabbitMQ, Seq)
# From repo root:
docker compose up -d
```

## Local Infrastructure (docker-compose)

| Service | URL |
|---|---|
| SQL Server | `localhost:1433` SA / `Brian_Dev_P@ss1` |
| RabbitMQ management | `localhost:15672` guest/guest |
| Redis | `localhost:6379` |
| Seq log viewer | `localhost:5341` |

## First-Time Setup

1. Install the EF Core CLI tool (once per machine):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. Set a real signing key before running Identity service. In `src/Services/Identity/Identity.API/appsettings.json` and `src/Services/Notifications/Notifications.API/appsettings.json`, replace `REPLACE_WITH_STRONG_SECRET_MIN_32_CHARS` with a 32+ character secret. All services must share the same key.

## Migrations

Each service has its own SQL Server database. Run migrations per-service:

```bash
cd Solutions/SampleSolution/CleanArchitectureSample
dotnet ef migrations add Init --project src/Services/Identity/Identity.Infrastructure --startup-project src/Services/Identity/Identity.API
dotnet ef database update          --project src/Services/Identity/Identity.Infrastructure --startup-project src/Services/Identity/Identity.API

dotnet ef migrations add Init --project src/Services/Calendar/Calendar.Infrastructure --startup-project src/Services/Calendar/Calendar.API
dotnet ef database update          --project src/Services/Calendar/Calendar.Infrastructure --startup-project src/Services/Calendar/Calendar.API
```

## Tests

```bash
dotnet test
```

`tests/Identity.Tests/` is the only test project scaffolded so far.

## Architecture

### Service Layout (36 projects)

```
src/
  Gateway/                     ← YARP reverse proxy, port 7000
  Services/
    Identity/     ports 7001   ← Auth, accounts, profiles, JWT, OpenIddict
    Calendar/     port  7002   ← Personal events, reminders, Holodex sync
    Creator/      port  7003   ← VTuber profiles, Q&A, fan art, memberships
    Community/    port  7004   ← Forums, badges, streaks, gamification
    Agency/       port  7005   ← CRM, contracts, tasks, wikis, OKRs
    Analytics/    port  7006   ← Channel snapshots, stream metrics
    Marketplace/  port  7007   ← Listings, orders, commissions
    Notifications/ port 7008   ← Email/push dispatch, SignalR hub
  Shared/
    Shared.Contracts/          ← Integration event records (no logic)
    Shared.Infrastructure/     ← BaseEntity, Result<T>, MediatR behaviours
```

Each service: `{Name}.Domain → {Name}.Application → {Name}.Infrastructure → {Name}.API`

### Dependency Flow

```
Domain (no deps)
  ↓
Application (MediatR, FluentValidation, EF Core abstractions)
  ↓
Infrastructure (EF Core SqlServer, MassTransit, Hangfire, Refit)
  ↓
API (ASP.NET Core, JWT, Scalar docs, Serilog)
```

Shared.Contracts has zero dependencies (pure records). Shared.Infrastructure provides base types used by all Domain/Application layers.

## Key Conventions

- **Central Package Management**: All NuGet versions in `Directory.Packages.props`. Never add `Version=` attributes to `.csproj` files. Notable pinned versions: `Mapster 10.0.0` + `Mapster.DependencyInjection 10.0.0` (not 7.x), `Scalar.AspNetCore 2.0.0` (requires explicit `using Scalar.AspNetCore;`), `Refit 7.1.2` (has a dev-advisory warning — harmless for local dev).
- **Entity PKs**: `Guid.CreateVersion7()` set in `BaseEntity` constructor — never set manually.
- **MediatR 12**: Pipeline delegates are `RequestHandlerDelegate<T>` — call as `next()` not `next(cancellationToken)`.
- **Result<T>**: Application handlers return `Result<T>.Success(value)` or `Result<T>.Failure(errors)` — no exceptions for business logic.
- **JWT claims**: Tokens embed `sub` (accountId), `profile_id`, `roles[]`. All downstream services validate against Identity service's signing key.
- **EF Core**: Application layer interfaces use `DbSet<T>` (EF Core ref in Application .csproj is intentional for the interface types + LINQ extensions).
- **MassTransit**: Integration events defined in `Shared.Contracts`. In Phase 1, consumers that need `IHubContext<NotificationHub>` live in `Notifications.API` (not Infrastructure) because the web SDK is required for SignalR types.
- **SignalR**: Hub at `/hubs/notifications` on Notifications service. Clients join group named by `profile_id`. JWT via query string `?access_token=` for SignalR connections.
- **Hangfire**: Calendar service runs `reminder-dispatch` job (Cron.Minutely). Dashboard at `/hangfire`.
- **Refit**: `IHolodexApiClient` interface (with Refit attributes) lives in Calendar.Application — registered in Calendar.Infrastructure DI.
- **File-scoped namespaces**, primary constructors, collection expressions (`[]`) — C# 13 style throughout.

## Phase Status

- **Phase 0** ✅ — Solution scaffold, Shared libraries, docker-compose, Gateway, Central Package Management
- **Phase 1** ✅ — Identity service (auth/profiles/JWT/OpenIddict), Calendar service (events/reminders/Holodex), Notifications service (SignalR hub, StreamGoLive consumer, reminder dispatch endpoint)
- **Phase 2** — Creator service, Community service (gamification, badges)
- **Phase 3** — Agency, Analytics, platform sync, SignalR Redis scale-out
- **Phase 4** — Marketplace, payments, AI assistant
- **Phase 5** — Caching, observability, rate limiting, performance
