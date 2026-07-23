# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Purpose

Brian is a VTuber/creator ecosystem backend built on Clean Architecture + microservices (ASP.NET Core .NET 9). The product covers three apps: Fan App, Creator App, and Agency/CRM App. Feature specs live in `Documentation/`.

## Solution Location

There is no single top-level solution. Each service is its own independent solution under `Solutions/`, with its own `.sln`, `Directory.Build.props`, and `Directory.Packages.props`:

```
Solutions/
  GatewayOrchestration/      ← Gateway (YARP reverse proxy), port 7000 — no Domain/Application/Infrastructure layers
  IdentitySolution/          ← Identity.API, port 7001
  CalendarSolution/          ← Calendar.API, port 7002
  CreatorSolution/           ← Creator.API, port 7003 (scaffolded only, Phase 2)
  CommunitySolution/         ← Community.API, port 7004 (scaffolded only, Phase 2)
  AgencySolution/            ← Agency.API, port 7005 (scaffolded only, Phase 3)
  AnalyticsSolution/         ← Analytics.API, port 7006 (scaffolded only, Phase 3)
  MarketplaceSolution/       ← Marketplace.API, port 7007 (scaffolded only, Phase 4)
  NotificationsSolution/     ← Notifications.API, port 7008 (no launchSettings.json yet — no confirmed dev port)
  HolodexSolution/           ← Holodex.API, port 7009
  TwitchSolution/            ← Twitch.API, port 7010
  YouTubeSolution/           ← YouTube.API, port 7011
  OnboardingSolution/        ← Onboarding.API, port 7012 — thin BFF, no Domain/Application/Infrastructure layers
  Libraries/SharedLibraries/ ← template copied into each new service's src/Shared/ when scaffolding a service
  ModerationSolution/        ← early-stage stub (Moderation.API + Moderation.Domain only, not wired into Gateway)
  Connectors/                ← empty, not started
```

`dotnet` commands run from inside whichever `Solutions/{Name}Solution/` directory you're working in — not from the repo root.

## Build & Run

```bash
# Build/restore a single service (repeat per solution you're touching)
cd Solutions/IdentitySolution
dotnet build
dotnet restore

# Run a service
dotnet run --project src/Identity/Identity.API/Identity.API.csproj   # :7001

# Start infrastructure (SQL Server, Redis, RabbitMQ, Seq) - from repo root
docker compose up -d
```

Ports match the list under Solution Location above. `Solutions/GatewayOrchestration/src/Gateway/appsettings.json`'s `ReverseProxy:Clusters` is the authoritative routing table for which service lives on which port and which `/api/*` paths it owns.

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

2. Each service that needs a DB/JWT reads `ConnectionStrings:DefaultConnection` and `Jwt:SigningKey` from config. `appsettings.json` only holds placeholders (`REPLACE_WITH_STRONG_SECRET_MIN_32_CHARS`, `REPLACE_WITH_YOUR_SA_PASSWORD`) — real values are supplied per-project via `dotnet user-secrets` (never committed). All services must share the same `Jwt:SigningKey`. Example for Identity:
   ```bash
   cd Solutions/IdentitySolution/src/Identity/Identity.API
   dotnet user-secrets set "Jwt:SigningKey" "<same 32+ char value across every service>"
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=Brian_Identity;User Id=sa;Password=Brian_Dev_P@ss1;TrustServerCertificate=true"
   ```
   Repeat the connection-string secret per service/database (`Brian_Calendar`, `Brian_Holodex`, `Brian_Twitch`, `Brian_YouTube`). Identity.API additionally needs `Google:ClientId`/`Google:ClientSecret`; Holodex.API additionally needs `Credentials:EncryptionKey` (a base64-encoded 32-byte AES-256 key).

## Migrations

Only Identity, Calendar, Holodex, Twitch, and YouTube have EF Core migrations so far, each against its own SQL Server database (`Brian_Identity`, `Brian_Calendar`, `Brian_Holodex`, `Brian_Twitch`, `Brian_YouTube`). Run from inside that service's own solution directory:

```bash
cd Solutions/IdentitySolution
dotnet ef migrations add Init --project src/Identity/Identity.Infrastructure --startup-project src/Identity/Identity.API
dotnet ef database update          --project src/Identity/Identity.Infrastructure --startup-project src/Identity/Identity.API
```

Same pattern for the others, e.g.:

```bash
cd Solutions/CalendarSolution
dotnet ef database update --project src/Calendar/Calendar.Infrastructure --startup-project src/Calendar/Calendar.API

cd Solutions/HolodexSolution
dotnet ef database update --project src/Holodex/Holodex.Infrastructure --startup-project src/Holodex/Holodex.API
```

## Tests

```bash
cd Solutions/IdentitySolution
dotnet test
```

`Solutions/IdentitySolution/tests/Identity.Tests/` is the only test project scaffolded so far.

## Architecture

### Service Layout

Each service (except Gateway and Onboarding, which are thin/no-layer) follows: `{Name}.Domain → {Name}.Application → {Name}.Infrastructure → {Name}.API`, plus its own local copy of `src/Shared/Shared.Contracts` and `src/Shared/Shared.Infrastructure`.

| Service | Port | Purpose |
|---|---|---|
| Gateway | 7000 | YARP reverse proxy routing `/api/*` to each service |
| Identity | 7001 | Auth, accounts, profiles, JWT, OpenIddict, Google OAuth |
| Calendar | 7002 | Personal events, reminders, Holodex-synced stream events |
| Creator | 7003 | VTuber profiles, Q&A, fan art, memberships (scaffolded, Phase 2) |
| Community | 7004 | Forums, badges, streaks, gamification (scaffolded, Phase 2) |
| Agency | 7005 | CRM, contracts, tasks, wikis, OKRs (scaffolded, Phase 3) |
| Analytics | 7006 | Channel snapshots, stream metrics (scaffolded, Phase 3) |
| Marketplace | 7007 | Listings, orders, commissions (scaffolded, Phase 4) |
| Notifications | 7008 | Email/push dispatch, SignalR hub |
| Holodex | 7009 | Holodex API-key storage, channel search/follow, video sync |
| Twitch | 7010 | Twitch integration (credential storage so far) |
| YouTube | 7011 | YouTube integration (credential storage so far) |
| Onboarding | 7012 | Sign-up/sign-in static pages; BFF-proxies to Identity |

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

- **Central Package Management**: each service solution manages its own NuGet versions in its own `Directory.Packages.props` (there is no single shared one) — never add `Version=` attributes to `.csproj` files. Notable pinned versions: `Mapster 10.0.0` + `Mapster.DependencyInjection 10.0.0` (not 7.x), `Scalar.AspNetCore 2.0.0` (requires explicit `using Scalar.AspNetCore;`), `Refit 7.1.2` (has a dev-advisory warning — harmless for local dev).
- **Entity PKs**: `Guid.CreateVersion7()` set in `BaseEntity` constructor — never set manually.
- **MediatR 12**: Pipeline delegates are `RequestHandlerDelegate<T>` — call as `next()` not `next(cancellationToken)`.
- **Result<T>**: Application handlers return `Result<T>.Success(value)` or `Result<T>.Failure(errors)` — no exceptions for business logic.
- **JWT claims**: Tokens embed `sub` (accountId), `profile_id`, `roles[]`. All downstream services validate against Identity service's signing key.
- **EF Core**: Application layer interfaces use `DbSet<T>` (EF Core ref in Application .csproj is intentional for the interface types + LINQ extensions).
- **MassTransit**: Integration events defined in `Shared.Contracts`. In Phase 1, consumers that need `IHubContext<NotificationHub>` live in `Notifications.API` (not Infrastructure) because the web SDK is required for SignalR types.
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
- **Phase 1** ✅ — Identity service (auth/profiles/JWT/OpenIddict), Calendar service (events/reminders/Holodex), Notifications service (SignalR hub, StreamGoLive consumer, reminder dispatch endpoint)
- **Phase 2** — Creator service, Community service (gamification, badges)
- **Phase 3** — Agency, Analytics, platform sync, SignalR Redis scale-out
- **Phase 4** — Marketplace, payments, AI assistant
- **Phase 5** — Caching, observability, rate limiting, performance
