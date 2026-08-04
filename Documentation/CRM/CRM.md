# Agency CRM: Contact/Company Records

**Purpose:** Give agency staff a CRM to track the agency's own creator roster and, later, brand/sponsor relationships — without leaving Brian for a spreadsheet or a third-party tool.

**Last Updated:** August 2026 | **Version:** 1.0

**Status:** Implemented (backend only), verified end-to-end via curl. No frontend yet — see Architecture.

---

## Goal

To add a CRM tool available for agents to use.

**Original guideline:** "Can we add Affine as the CRM tool."

## Decision

**AFFiNE was ruled out** after research: it's an open-source Notion/Miro-style notes+whiteboard workspace (MIT-licensed editor, but source-available/restrictive-licensed backend for its "Enterprise Edition" features) — it has no contacts, deals, or pipeline objects, so it isn't actually a CRM. Rather than force-fit it, the requirements were defined from scratch with the user first.

| Concern | Choice |
|---|---|
| Build vs. embed a third-party tool | **Build natively** in the existing `AgencySolution`, matching every other service's Clean Architecture pattern — day-one scope (plain contact/company CRUD) didn't justify standing up a heavy external tool the way Stoat was for chat (see [ChatWindow.md](../Calendar/ChatWindow.md) for that contrast) |
| CRM priority | The agency's own **creator roster first**, brands/sponsors later (phased) |
| Day-one core workflow | **Contact/company records only** — explicitly not deal/pipeline tracking, not notes/activity history, not tasks/reminders (all considered, all declined for now) |
| Integration | **Standalone** — uses Identity for JWT auth like every other service, no data linkage to existing Creator/Profile/Calendar/Marketplace records yet |
| Auth | Gated by the existing `UserRole.AgencyAdmin` — no new role needed |

**Found during research, explicitly out of scope for now**: `Documentation/OriginalResearch/CRM Notes.txt` describes a much bigger long-term vision (scaling to Hololive/Nijisanji-size agencies, manager access to a VTuber's public persona, milestone/reward tracking, SocialBlade integration, agency-wide event/studio-time scheduling). None of that is built yet — treat it as future-phase background only. Also found: `Documentation/Identity/UserProfiles.md` describes richer roles ("Agency Owner", "Personal Manager", "Business/Corporate Account") that don't actually exist in the `UserRole` enum (only `Fan/Creator/Manager/AgencyAdmin` are implemented) — a pre-existing spec/code gap, not something this feature fixes.

## Architecture

Standard four-layer service (`Agency.Domain` → `Agency.Application` → `Agency.Infrastructure` → `Agency.API`), built out on top of what was previously a scaffold-only `AgencySolution`.

- **`Agency.Domain`**: `Contact` (FirstName, LastName, Email, Phone, Title, `ContactCategory` enum: `Roster`/`Sponsor`/`Other`, nullable `CompanyId`) and `Company` (Name, `CompanyCategory` enum: `Sponsor`/`Partner`/`Other`, Website). The category enums exist specifically so staff can filter by the phased roster-then-sponsors scope from day one, without needing separate entities per category. Nullable FK, not a join table — simple is right at this scale; revisit if multi-company contacts become real. Hard-delete, not soft-delete — no entity anywhere in the repo uses a soft-delete pattern.
- **`Agency.Application`**: standard MediatR CRUD (`Features/Contacts/`, `Features/Companies/`, each with Create/Update/Delete commands and GetAll/GetById queries), `Result<T>` returns, FluentValidation validators.
- **`Agency.API`**: `POST/GET/PUT/DELETE /api/agency/contacts` and `/api/agency/companies`, both `[Authorize(Roles = "AgencyAdmin")]`. No CORS — no frontend consumer yet. Gateway route (`/api/agencies/{**catch-all}` → `:7005`) already existed from the original scaffold, unused until now.

**Auth gotcha found while verifying live** (documented in `CLAUDE.md` too, since it affects any future service adding role-gated endpoints): `TokenService` emits role grants under a custom `"roles"` JWT claim type, but ASP.NET Core's default JWT-bearer claim mapping auto-renames it to the standard role claim type on validation — so `[Authorize(Roles = "...")]` works with **zero extra configuration**. The seemingly-obvious fix of explicitly setting `TokenValidationParameters.RoleClaimType = "roles"` is actually wrong and silently breaks authorization, since the raw `"roles"`-typed claim no longer exists once the automatic remapping has renamed it away. Caught via a temporary debug claims-dump endpoint (removed before commit) — this was the first service in the repo to use role-based `[Authorize]` at all, so there was no prior example to copy.

**Explicitly out of scope for now**: any Agency.Web frontend (this is backend-only, verified via curl/Scalar — no UI exists to consume it yet), deal/pipeline entities, notes/activity-history, tasks/reminders, linkage to Identity's Creator/Profile records, anything from the bigger CRM Notes.txt vision.

## Verification

Curl-based, no frontend to verify:
1. Registered a real account, then created a profile with `role: AgencyAdmin` to mint a usable JWT (role isn't settable at registration — `RegisterAccountCommandHandler` hardcodes `Fan` — but `CreateProfileCommand` accepts any `UserRole`).
2. Full CRUD round-trip verified on both Contacts and Companies (create → read → update → delete), including the `CompanyId` link between them.
3. Confirmed a `Fan`-role token gets `403` and an unauthenticated request gets `401` — role gating actually works, not just present in code.

## Next steps (not started)

- Agency.Web frontend to actually use this from a browser.
- Deal/pipeline tracking once the roster-first phase is proven out.
- Linking Contacts to real Identity Creator/Profile records instead of freeform entries.
