# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Purpose

Brian is a VTuber/creator ecosystem backend built on Clean Architecture + microservices (ASP.NET Core .NET 9). The product covers three apps: Fan App, Creator App, and Agency/CRM App. Feature specs live in `Documentation/`.

## Solution Location

There is no single top-level solution. Each service is its own **self-contained solution** under `Solutions/`, with its own `.sln`, `Directory.Packages.props` (Central Package Management), `Directory.Build.props` (`Nullable`/`ImplicitUsings` enable, `LangVersion latest`), and its own copies of `Shared.Contracts`/`Shared.Infrastructure` (see Key Conventions — these are duplicated per solution, not referenced across solution boundaries):

```
Solutions/
  GatewayOrchestration/     ← Gateway.csproj only (YARP reverse proxy), :7000
  IdentitySolution/         ← Identity.{Domain,Application,Infrastructure,API}, :7001
  CalendarSolution/         ← Calendar.{Domain,Application,Infrastructure,API}, :7002
  CreatorSolution/          ← scaffold only, :7003 (see Phase Status)
  CommunitySolution/        ← scaffold only, :7004
  AgencySolution/           ← scaffold only, :7005
  AnalyticsSolution/        ← scaffold only, :7006
  MarketplaceSolution/      ← scaffold only, :7007
  NotificationsSolution/    ← Notifications.API (SignalR hub, consumers), :7008
  HolodexSolution/          ← Holodex platform sync, :7009
  TwitchSolution/           ← Twitch platform sync, :7010
  YouTubeSolution/          ← YouTube platform sync, :7011
  ModerationSolution/       ← Moderation.API (async content moderation), :7012
  Libraries/SharedLibraries/ ← reference copy of Shared.Contracts/Shared.Infrastructure (not a runnable solution)
  Connectors/               ← empty, reserved
```

All `dotnet` commands run from inside the relevant `Solutions/<Name>Solution/` directory, not the repo root.

## Build & Run

```bash
# Build one service:
cd Solutions/CalendarSolution
dotnet build
dotnet restore

# Run it:
dotnet run --project src/Calendar/Calendar.API/Calendar.API.csproj   # :7002

# Start infrastructure (SQL Server, Redis, RabbitMQ, Seq) — from repo root:
docker compose up -d
```

There's no single `dotnet build` that covers every service — build/run each solution independently. Ports:

| Service | Solution | Port |
|---|---|---|
| Gateway (YARP) | `GatewayOrchestration` | 7000 |
| Identity | `IdentitySolution` | 7001 |
| Calendar | `CalendarSolution` | 7002 |
| Creator | `CreatorSolution` | 7003 |
| Community | `CommunitySolution` | 7004 |
| Agency | `AgencySolution` | 7005 |
| Analytics | `AnalyticsSolution` | 7006 |
| Marketplace | `MarketplaceSolution` | 7007 |
| Notifications | `NotificationsSolution` | 7008 |
| Holodex sync | `HolodexSolution` | 7009 |
| Twitch sync | `TwitchSolution` | 7010 |
| YouTube sync | `YouTubeSolution` | 7011 |
| Moderation | `ModerationSolution` | 7012 |

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

2. Secrets are **not** stored in `appsettings.json` — those files only have `REPLACE_WITH_...` placeholders committed to git. Real values live in `dotnet user-secrets` per API project. Set them once per machine:
   ```bash
   cd Solutions/IdentitySolution/src/Identity/Identity.API
   dotnet user-secrets set "Jwt:SigningKey" "<32+ char secret, same value across every service>"
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=Brian_Identity;User Id=sa;Password=Brian_Dev_P@ss1;TrustServerCertificate=true"
   dotnet user-secrets set "Google:ClientId" "<from Google Cloud Console>"
   dotnet user-secrets set "Google:ClientSecret" "<from Google Cloud Console>"
   ```
   Repeat `Jwt:SigningKey` (same value) and `ConnectionStrings:DefaultConnection` (service-specific database name, e.g. `Brian_Calendar`) for every other API project that needs them (Calendar, Notifications, etc.). `dotnet user-secrets list` (run inside the `*.API` project directory) shows what's already set.

## Migrations

Each service with a database has its own SQL Server database and its own migrations, run from inside that service's solution directory:

```bash
cd Solutions/IdentitySolution
dotnet ef migrations add Init --project src/Identity/Identity.Infrastructure --startup-project src/Identity/Identity.API
dotnet ef database update          --project src/Identity/Identity.Infrastructure --startup-project src/Identity/Identity.API

cd ../CalendarSolution
dotnet ef migrations add Init --project src/Calendar/Calendar.Infrastructure --startup-project src/Calendar/Calendar.API
dotnet ef database update          --project src/Calendar/Calendar.Infrastructure --startup-project src/Calendar/Calendar.API
```

`ModerationSolution` has no database (stateless queue consumer) — no migrations needed there.

## Tests

```bash
cd Solutions/IdentitySolution
dotnet test
```

`Solutions/IdentitySolution/tests/Identity.Tests/` is the only test project with actual tests. Other solutions have an empty `tests/` folder scaffolded but no test project in it yet.

## Architecture

### Service Layout

Every service solution follows the same internal shape:

```
Solutions/<Name>Solution/
  <Name>Solution.sln
  Directory.Packages.props   ← Central Package Management for this solution
  Directory.Build.props      ← Nullable/ImplicitUsings enable, LangVersion latest
  src/
    <Name>/
      <Name>.Domain/         ← entities, enums, no dependencies
      <Name>.Application/    ← MediatR handlers, FluentValidation, EF Core abstractions
      <Name>.Infrastructure/ ← EF Core SqlServer, MassTransit, Hangfire, Refit, consumers
      <Name>.API/            ← ASP.NET Core, JWT, Scalar docs, Serilog
    Shared/
      Shared.Contracts/      ← Integration event records (no logic)
      Shared.Infrastructure/ ← BaseEntity, Result<T>, MediatR behaviours
  tests/
```

`GatewayOrchestration` is the exception — it's a single YARP reverse-proxy project (`Gateway.csproj`) with no Domain/Application/Infrastructure split.

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

- **No cross-solution project references**: `Shared.Contracts` and `Shared.Infrastructure` are **duplicated** into every service solution's `src/Shared/` — there is no project reference across solution boundaries. `Solutions/Libraries/SharedLibraries` holds a reference copy. When adding a new integration event (or changing a shared base type), copy the same file into every solution that needs it — the compiler won't catch a solution you forgot.
- **Central Package Management**: All NuGet versions in each solution's own `Directory.Packages.props`. Never add `Version=` attributes to `.csproj` files. Notable pinned versions: `Mapster 10.0.0` + `Mapster.DependencyInjection 10.0.0` (not 7.x), `Scalar.AspNetCore 2.0.0` (requires explicit `using Scalar.AspNetCore;`), `Refit 7.1.2` (has a dev-advisory warning — harmless for local dev).
- **Feature folder naming**: name feature folders after the use case/area (`Events`, `Backgrounds`, `Reminders`), not after the entity itself. A folder named identically to its entity (e.g. `Features/CalendarBackground/...` next to entity `CalendarBackground`) creates a real C# namespace/type ambiguity — the namespace segment shadows the `using`-imported type.
- **Entity PKs**: `Guid.CreateVersion7()` set in `BaseEntity` constructor — never set manually.
- **MediatR 12**: Pipeline delegates are `RequestHandlerDelegate<T>` — call as `next()` not `next(cancellationToken)`.
- **Result<T>**: Application handlers return `Result<T>.Success(value)` or `Result<T>.Failure(errors)` — no exceptions for business logic.
- **JWT claims**: Tokens embed `sub` (accountId), `profile_id`, `roles[]`. All downstream services validate against Identity service's signing key (same `Jwt:SigningKey` user-secret value in every service that authenticates requests).
- **EF Core**: Application layer interfaces use `DbSet<T>` (EF Core ref in Application .csproj is intentional for the interface types + LINQ extensions).
- **MassTransit**: Integration events defined in `Shared.Contracts`. Consumers that need `IHubContext<NotificationHub>` live in `Notifications.API` (not Infrastructure) because the web SDK is required for SignalR types. A service that only consumes/publishes events with no HTTP API of its own (e.g. `Moderation.API`) can still be a minimal ASP.NET Core host — just for the MassTransit host lifetime and a `/health` endpoint.
- **SignalR**: Hub at `/hubs/notifications` on Notifications service. Clients join group named by `profile_id`. JWT via query string `?access_token=` for SignalR connections.
- **Hangfire**: Calendar service runs `reminder-dispatch` job (Cron.Minutely). Dashboard at `/hangfire`. Note: Calendar.API currently crashes at startup (unhandled exception) if SQL Server isn't reachable when this job registers — start `docker compose up -d` before Calendar.API.
- **Refit**: `IHolodexApiClient` interface (with Refit attributes) lives in Holodex.Application — registered in Holodex.Infrastructure DI. `SearchChannelsAsync`'s `search` parameter is optional; omitting it returns Holodex's default browse list instead of requiring a name.
- **Credential encryption**: External credentials (e.g. Holodex API keys) are encrypted at rest with AES-256-GCM via `ICredentialEncryptionService` (`Holodex.Infrastructure/Services/AesCredentialEncryptionService.cs`), keyed by `Credentials:EncryptionKey` config/user-secrets — not Base64.
- **Static files**: services that serve static HTML/JS (Identity, Calendar, Onboarding) set `Cache-Control: no-cache, no-store, must-revalidate` on `UseStaticFiles()` so browsers don't serve a stale page after a file changes on disk.
- **File-scoped namespaces**, primary constructors, collection expressions (`[]`) — C# 13 style throughout.

## Recent Changes

- **Onboarding service extracted**: `Solutions/OnboardingSolution/` (Onboarding.API, port 7012) is a thin BFF that hosts `login.html`/`profile-setup.html`, moved out of `Identity.API/wwwroot`. It proxies `POST /api/auth/login` and `POST /api/profiles` server-to-server to Identity.API via `IHttpClientFactory` (no CORS needed — the browser only ever talks to Onboarding's own origin). Identity's Google OAuth callback redirects to `Onboarding:BaseUrl` for profile setup instead of a same-origin relative path. `holodex-setup.html` was intentionally left behind, unlinked, in `Identity.API/wwwroot` for later reintroduction.
- **VTuber follow flow moved to its own page**: `Solutions/CalendarSolution/.../wwwroot/holodex-follow.html` replaces the old inline picklist in `calendar.html`. It auto-loads a browse list from Holodex on load, supports searching by name (selections persist across searches, keyed by `channelId`), and a "Finished" button saves the selection and returns to `calendar.html`. The Holodex section on `calendar.html` is now a `<fieldset>` groupbox that displays the actual list of followed VTubers, not just a count.
- **Static file caching fixed**: `Calendar.API`, `Identity.API`, and `Onboarding.API` now set `Cache-Control: no-cache, no-store, must-revalidate` on static files — previously a browser could keep serving a stale cached HTML/JS page after the file changed on disk.

## Phase Status

- **Phase 0** ✅ — Solution scaffold, Shared libraries, docker-compose, Gateway, Central Package Management
- **Phase 1** ✅ — Identity service (auth/profiles/JWT/OpenIddict), Calendar service (events/reminders/Holodex/background image upload with async moderation pipeline), Notifications service (SignalR hub, StreamGoLive consumer, reminder dispatch endpoint, calendar-background-ready push)
- **Unplanned/ahead of the phase list, but real**: Holodex/Twitch/YouTube platform-sync services (`HolodexSolution`, `TwitchSolution`, `YouTubeSolution`) and the Moderation service (`ModerationSolution`) all have working implementations, even though the phases below haven't formally started.
- **Phase 2** — Creator service, Community service (gamification, badges) — solutions are scaffolded (`Program.cs` + shared boilerplate only), no feature code yet
- **Phase 3** — Agency, Analytics, SignalR Redis scale-out — scaffolded only
- **Phase 4** — Marketplace, payments, AI assistant — scaffolded only
- **Phase 5** — Caching, observability, rate limiting, performance
