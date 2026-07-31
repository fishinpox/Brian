# Calendar Chat Window

**Purpose:** Add an embedded chat window to the Calendar App so users can stay in Brian instead of leaving for Discord/Slack/Teams, increasing app stickiness.

**Last Updated:** July 2026 | **Version:** 1.0

**Status:** Implemented (backend + frontend), verified end-to-end over real HTTPS. One local-dev-only setup gotcha, documented below — not expected to occur in a real deployment.

---

## Goal

Add a chat window to the Calendar App so users can stay in the Calendar App and don't have to use Discord, Slack, or Microsoft Teams. This increases the stickiness of the Calendar App.

**Original guidelines:** use the open source [Stoat](https://github.com/stoatchat) (formerly Revolt) from GitHub to perform the chatting; make it a new solution the Calendar app can interact with; make the UI similar to [discord.com](https://discord.com/).

## Decision

| Concern | Choice |
|---|---|
| Chat platform | **Stoat**, self-hosted, deployed unmodified |
| UI approach | **Embed Stoat's own web client** in an iframe — not a custom-built UI |
| Backend | New `Solutions/ChatSolution`/`Chat.API` (:7013) — thin account-provisioning bridge, not a message relay |
| Voice/video | Out of scope — text-only chat |
| Account linking | Chat.API auto-creates a Stoat account per Brian profile, fully server-side, no manual admin step |
| First-login UX | One manual login into the embedded client's own login form, first time only — accepted target, not a fallback |

## Why this combination

The goal isn't "a chat feature" in the abstract — it's capturing Discord's audience by making the tool as close to a 1:1 knowledge-transfer as possible. Stoat's UI already maps almost directly onto Discord's (servers, text/voice channels, roles, emoji reactions), which no alternative (Rocket.Chat, Fluxer, Zulip, Matrix) matches as closely. Self-hosting it unmodified and only interacting over its network API is the standard low-AGPL-risk pattern (Stoat is AGPL-3.0). Embedding Stoat's actual client — rather than building a custom UI against its API — gets guaranteed, maintained-upstream Discord-parity for zero ongoing UI-build effort, at the cost of not matching Calendar.Web's own design system and no control over its UI.

**Alternatives considered and ruled out:** Rocket.Chat (MIT, but capped at 100 concurrent users on the free tier), Fluxer (AGPL-3.0, early/alpha, no documented admin API), Zulip (Apache 2.0, topic-threaded UX diverges from Discord), Matrix/Synapse (Apache 2.0, most complex to self-host).

## Architecture

### 1. Self-hosted Stoat infrastructure

Stoat's real self-hosted footprint is **15 containers**, not a small add-on — `database` (MongoDB), `redis` (KeyDB), `rabbit` (RabbitMQ), `minio` + `createbuckets` (file storage — not cleanly excludable, `autumn`/`crond` hard-depend on it), `caddy` (reverse proxy), `api`, `events`, `autumn` (file server), `january` (metadata/image proxy), `gifbox` (Tenor proxy), `crond` (task daemon), `pushd` (push notifications), `web` (the actual client). Only `voice-ingress` and `livekit` are excluded — safe, since nothing else depends on them and this is text-only chat.

Vendored at `Solutions/ChatSolution/self-hosted/` (cloned from `stoatchat/self-hosted`, nested `.git` stripped so it's plain tracked content) and run via its **own** `docker compose` invocation — deliberately **not** merged into the repo-root `docker-compose.yml`, given the size difference. Config generated via its own `generate_config.sh localhost` script (reverse-proxy mode, no video), which exposes Caddy on host port `8880` via a generated `compose.override.yml` to avoid binding the standard 80/443 ports.

### 2. `Solutions/ChatSolution` (Chat.API)

Standard four-layer service layout, scaffolded via `dotnet new sln`/`dotnet sln add`. `Chat.Domain.Entities.ChatAccountLink` stores `ProfileId` → Stoat `StoatUserId`/`StoatUsername`/`StoatEmail`/`EncryptedStoatPassword` (AES-256-GCM, same pattern as Holodex's `ICredentialEncryptionService`).

`EnsureChatAccountCommand` (`POST /api/chat/account`, `[Authorize]`) is idempotent: if a link already exists for the caller's profile, it decrypts and returns the stored credentials with zero Stoat network calls. Otherwise it runs the full provisioning sequence against `IStoatApiClient` (a Refit interface):

1. `POST /auth/account/create` `{email, password}` — a synthetic `chat-{profileId:N}@chat.internal.brian` email and a random password. No email verification or captcha needed (both disabled on this instance).
2. `POST /auth/session/login` `{email, password, friendly_name}` — returns a real session token directly.
3. `POST /onboard/complete` `{username}` (with `x-session-token` header) — new accounts land in an onboarding state until this completes; Stoat auto-assigns a discriminator on username collision, so no retry logic is needed.

The response (`{username, email, password}`) is handed to the frontend for the one-time manual login step.

Gateway route: `chat-route`/`chat-cluster` (`/api/chat/{**catch-all}` → `https://localhost:7013`) in `GatewayOrchestration`'s `appsettings.json`.

### 3. Calendar.Web changes

- **Refresh-token support** was bundled into this work since chat is a long-lived-session feature and Calendar.Web had zero refresh logic despite the backend supporting it. `Identity.API`'s Google callback and `Onboarding.API`'s `login.html`/`profile-setup.html` now propagate `&refreshToken=` alongside the access token; `Calendar.Web/src/lib/auth.ts` gained a single-flight `getValidAccessToken()` (silently refreshes an expiring token, deduped so concurrent callers can't race the backend's refresh-token rotation into a reuse-detection revoke), used by both `apiClient.ts` and `signalr.ts`.
- `src/config/platforms.ts`: `CHAT_API` (`https://localhost:7013`) and `STOAT_WEB_URL` (`http://localhost:8880`, the self-hosted instance's own web client).
- `src/lib/queries/chatQueries.ts`: `useChatAccount(enabled)` — wraps `POST /api/chat/account`.
- `src/components/ChatPanel.tsx`: a slide-in panel (toggled from a "💬 Chat" button next to `FontPicker` in `App.tsx`'s top bar). On open: fetches the account, shows a dismissible banner with the login email + password, and renders `<iframe src={STOAT_WEB_URL}>`.

## Known issue: local-dev-only login friction (not expected in production)

Stoat's own `generate_config.sh` bakes an absolute `https://localhost` (no port) into `.env.web`/`stoat.json`, regardless of what port its Caddy container actually binds to. On this repo's non-standard `:8880` local setup, that breaks the embedded client's own login until those two files are manually corrected to `http://localhost:8880/...` and the `web` container is recreated (its `inject.js` entrypoint re-bakes `VITE_*` env vars into the served bundle on every container start — no image rebuild needed) with any stale browser service-worker cache cleared.

Even after that fix, the pre-built `for-web` client image (labeled `v0.10.0`) talking to the pinned `v0.13.8` backend showed a further quirk **specific to the non-HTTPS local setup**: login API calls succeeded and returned a genuinely valid session (independently confirmed via direct API calls with the same token), but the client's own internal state machine marked the session invalid and wouldn't proceed past its login screen. **Confirmed fixed by using real HTTPS on standard ports** (Caddy's own auto-provisioned local-CA certificate, trusted once via Windows' certificate store) — logging in over `https://localhost/` worked cleanly and reached the real app. Since the actual target deployment is a real domain with real HTTPS (matching Stoat's own assumed shape), this is not expected to be an issue in production — it only surfaced because of the local HTTP-on-a-nonstandard-port workaround used for this repo's dev setup.

## Known gap: port collision

`Moderation.API` and `Onboarding.API` both default to `:7012`/`:5012` — a pre-existing collision unrelated to this feature, noticed while assigning Chat.API's port (`:7013`). A fix (moves Moderation to `:7014`/`:5014`) is pending on branch `claude/confident-roentgen-880bd0`, not yet merged.
