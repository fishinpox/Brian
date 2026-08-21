# MASTER SPEC — All-in-One Creator Platform
**Version:** 1.2 · August 2026 · Compiled from full product planning sessions + merged planning notes
**Audience:** the development team and any AI assistant (Claude) working on this project. This file is the single source of truth for what the product is and does. Where anything conflicts with older notes, THIS FILE WINS.
**v1.1 changes:** merged role-based app model (Fan / Creator-VTuber / Manager / Agency), VTuber & agency operations, full productivity core, monthly brand-pack delivery + rarity system, friends/social non-negotiables, anonymous Q&A, dual-mode commission marketplace, healthy-fandom principles, and distilled Patreon/Carrd feature references. Locked decisions and MVP scope are UNCHANGED.
**v1.2 changes:** corrected the stack in Locked Decisions (§3) and every other reference to match the actual implementation already underway — **SQL Server**, not PostgreSQL (matching every other Brian microservice), and **React (Vite)**, not Next.js, for the frontend, per `FrontEndTechnologyChoices.md`'s already-decided Calendar frontend stack (FullCalendar, Motion, Tailwind CSS, TanStack Query, `@microsoft/signalr`). No product-scope changes.

---

## 1. Product Vision

One platform, one server, one identity graph, serving streamers, VTubers, creators, their teams/agencies, and their audiences. A fan is simultaneously a **member, customer, attendee, friend, and community participant** — one record, every context. The consumer hook is a streamer-following calendar (Holodex-style functionality); around it sits creator monetization (Patreon/Skeb/VGen-style functionality), creator pages (Carrd-style), Discord-style community, a Notion-style workspace + CRM scaling from solo creator to hololive/nijisanji-class agency, OS-level brand customization packs, convention/event operations, and small-business workforce + point-of-sale tools.

**Guiding rules:**
- Match the *functionality* of referenced products; never clone them 1:1.
- **Modular toggles everywhere:** every role can enable/disable feature sections (team tools, ADHD tools, monetization, community) so no user is overwhelmed by the full platform.
- **Healthy-fandom by design:** the app fosters genuine, bounded interaction — mutual value without the illusion of personal friendship. Fans feel seen; creators manage time/energy sustainably.

### Differentiators (features no referenced product has)
1. **Unified identity graph** — membership tier = chat role = CRM stage = commission history = loyalty record = friend graph.
2. **Single availability engine** — stream schedule, bookable slots, tasks, studio time, and staff shifts computed from one source.
3. **LLM-native input** — natural language ("collab with X next Tues 8pm, members-only, announce in #general") creates the event, the gate, and the announcement in one pass. LLM output is *proposed structured actions* validated server-side against real permissions — never directly executed (see §11).
4. **Cross-service digest** — one notification stream: going-live alerts, commission updates, chat mentions, CRM tasks, Q&A answers.
5. **Healthy-fandom architecture** — boundary tools, peer-community emphasis, and wellness features as first-class product surface, not an afterthought.

---

## 2. Context & Constraints

- **Team:** 2 people. Dev 1 = senior software architect, strongest in C#. Dev 2 = product/ideas, writes specs and drives AI tooling.
- **Budget:** near zero. Unavoidable costs: Apple dev account $99/yr, Google Play $25 one-time, Windows code-signing certificate ~$300–500/yr, VPS/hosting ~$100–300/mo at start.
- **Method:** AI-assisted development (Claude Code). Open source is used as *functional reference or federated service*, not transpiled into another language.
- **Timeline:** MVP beta target end of August 2026. Everything beyond MVP scope (§12) is sequenced after.

---

## 3. Locked Decisions

| Decision | Value |
|---|---|
| Stack | **C# server + TypeScript frontend.** ASP.NET Core 9 + SQL Server + SignalR backend, matching every other Brian microservice; React (Vite) on every screen — see `FrontEndTechnologyChoices.md` for the Calendar UI's specific frontend stack (FullCalendar, Motion, Tailwind CSS, TanStack Query, `@microsoft/signalr`). |
| MVP cut | Calendar + memberships + gated content (§12). v1.1 additions do NOT expand the MVP. |
| Infrastructure | Architect decides per component; spec work must present managed-vs-self-hosted options with costs. |
| Payments | Stripe / Stripe Connect for everything. Never build payment handling. |
| Voice/streams | Self-hosted LiveKit (.NET server SDK; JS client SDKs). |
| Purchases on iOS | All digital purchases go through **web checkout**, never Apple in-app purchase. iOS app is a viewer/library. |
| Revenue model | Fan apps free. 15% + processing on digital sales/memberships; 5–8% on physical merch; tickets ~2% + $0.99; Business Manager per-location SaaS; storage subscriptions for agencies (§10). |
| Content protection | "Private" = access-gated (signed URLs + entitlements), not DRM. True DRM is a deliberate later decision if ever needed. |

---

## 4. Roles & Relationships

- **Fan:** free consumer; follows, subscribes, attends, participates, befriends other fans.
- **Friend:** any two users can connect; unlocks event sharing + group chats (§6.1, non-negotiable).
- **Creator / VTuber:** individual talent. Supports **multiple switchable profiles/personas** (private life, work, one or more public personas) with separate calendars, boundaries, and audiences.
- **Artist:** marketplace seller of assets/commissions (may also be a Creator).
- **Manager:** oversees creators without a full agency. **1 manager ↔ many creators AND 1 creator ↔ many managers.** Scope: a manager can schedule/modify/update anything on a talent's public persona — page (Carrd-style site), schedules, content releases — per granted permissions. One consistent manager interface whether the talent is independent or agency-signed.
- **Agency:** enterprise tier managing many talents + staff (hololive/nijisanji scale target).
- **Business:** shops/restaurants/service companies using workforce + membership + POS tools.
- **Venue:** convention/event operators.
- **Partner:** hardware/brand companies (Hyte-class, §7).

**Cross-role interactions (by design):** fan RSVPs feed the creator's calendar; creator requests route to managers for approval; agency approvals cascade to managers then talents; manager edits publish to fan-facing pages.

**Frontend cross-references:** lightweight team invite/roster UI (email-based invite, per-member private-schedule visibility) is speced against this role model in `USER-STORY-team-sharing.md` — no new permission scheme, just a UI layer on the Manager relationship above. The manager-can-edit-a-locked-event exception in `USER-STORY-folder-lock-settings.md` also derives from this section's manager scope.

---

## 5. The Nine Programs

### 5.1 Core Server (invisible backbone) — C# / ASP.NET Core / SQL Server / SignalR / Redis
- Accounts, login, 2FA; profiles/personas; friend graph; OAuth provider registry (Twitch, YouTube/Google first; Spotify, Steam, SoundCloud, Bluesky/Mastodon, etc. as plugins behind one interface: authorize / refresh / fetch-events / render-filter).
- **Entitlements ledger** — the heart. Ownership of: memberships (with per-tier feature flags), theme/brand packs, monthly pack deliveries, tickets, digital goods, commission deliveries, loyalty status, VIP status.
- Calendar/event engine (ingestion-fed events + manual events + friend events + holiday layers: religious + government calendars, user-toggleable per side; custom date ranges, e.g., Lunar New Year).
- Payments via Stripe + Stripe Connect (subscriptions monthly/annual, one-time purchases, tips, commissions with escrow-style hold, tickets, merch orders with shipping addresses, creator payouts, tax/KYC via Connect).
- Notification fan-out (queue-based): SignalR to open apps; FCM/APNs/WebPush to closed apps; email. Per-event-type routing for custom alerts (§5.6).
- Chat backbone (SignalR): channels, DMs, group chats, roles/permissions, presence, typing, read states, file sharing.
- **Unified inbox** service: tasks, messages, Q&A, mentions, approvals in one queryable stream.
- Gamification engine: badges, points, streaks, levels, loyalty rewards, unlockables (emotes, previews, priority Q&A slots) — creator-configurable, rewards diverse/healthy engagement.
- Anonymous Q&A pipeline (§6.3); commission marketplace engine (§6.2); milestone/stats service (§5.7 Agency).
- Invite-link service: one-click join/import links (Discord-style) for events, group chats, communities — works for non-users (link → landing page → import on signup).
- Partner & venue dashboards' API; audit logs; signed-license issuing; driver/software distribution registry; per-account cloud-storage quotas.

### 5.2 Web App (free, everyone) — TypeScript / React (Vite)

**Frontend cross-reference:** the Calendar surface's specific frontend stack (React + Vite, FullCalendar, Motion, Tailwind CSS, TanStack Query, `@microsoft/signalr`) is decided in `FrontEndTechnologyChoices.md` — that document is the authoritative frontend-tech reference for this section, superseding any framework named elsewhere in this file.
- Streamer calendar: live-now grid, **multiview** (watch several streams at once), upcoming schedule, archive history, clip browsing, browse/discover creators, follow, custom watch lists, personal-calendar integration.
- Per-service toggle/filter for connected OAuth providers; per-creator notification levels.
- **Personalized feed & daily digest:** creator content + fan-generated highlights (clips, art, discussions); digest summaries to reduce doom-scrolling.
- Creator pages + stores; membership checkout (tiers with creator-defined benefits, free-follow tier, annual billing, discounts/special offers); one-time purchases; collections (content organized by series/theme); scheduled drops; newsletters (opt-in email from creators).
- Commission requests (both modes, §6.2); asset marketplace browsing (VGen/Pixiv-style).
- Community: forums + topic groups ("Lore," "Fan Art," "Off-Topic"), text channels, fan walls/shoutouts, co-watching rooms, discovery of similar fans, virtual/in-person meetup coordination.
- Fan creation hub: upload/share fan art, clips, covers, memes; creator approval workflow; "Featured Fan Content" spotlights; YouTube-highlight-channel program (fans clip, creator endorses).
- Interactive tools: polls (short + long term) and votes on stream ideas/games/schedule slots (fan-voting on tentative times/topics), Q&A submission (batched), reactions.
- Anonymous safe Q&A (§6.3); virtual gifts with transparency on usage.
- Gamification surface: streak tracking ("caught 5 streams this month"), goals ("watch X collabs"), badges, unlockables.
- Content library: replays, exclusive clips, member archives, downloadables (wallpapers, lore docs, playlists).
- Event pages, RSVPs with limited-spot exclusivity (watch parties, meet-and-greets, Q&As, lore deep-dives); floor-plan viewer (checked-in ticket holders); panel schedule with per-panel follow/block.
- **Wellness & boundary tools:** usage trackers, break prompts, healthy-fandom reminders, creator-is-a-person notices, community guidelines; fan-side notification controls.
- Support chat; account library ("everything I own"); world clocks (§6.1); in-app branded toasts from creator brand kits.

### 5.3 Desktop App (free, fans + heavy creator/manager workflow) — same TypeScript app wrapped (Electron on Mac; Tauri acceptable on Windows)
- Everything in Web App, plus: native desktop toasts with the creator's custom stinger/sound; offline access to owned/gated content and monthly packs; deep links into voice rooms; multi-view stream management; advanced dashboards, whiteboards, knowledge bases for creator/manager/agency roles; one-click "apply this month's image as desktop wallpaper" (hands off to Agent).

### 5.4 Mobile App (free, fans/attendees) — React Native / Expo (TypeScript) + small Swift/Kotlin modules
- Everything in Web App, plus: OS push; home-screen widgets; natural-language quick actions; rotating-barcode tickets (~30 s refresh); BLE proximity for booth check-ins and in-store member recognition (beacons broadcast IDs; server validates; QR fallback always); offline floor plans/tickets/panel schedules with sync; staff Scanner mode (camera scanning, offline store-and-forward queue).
- Brand-pack surface per OS limits (§8.2): wallpapers (one-click apply where OS allows; save-and-guide on iOS), widgets, app icons, in-app sounds; **ringtones + text-notification tones on Android**; iOS in-app sounds only.
- Location-based triggers for reminders (coarse, opt-in).

### 5.5 Creator Studio (creators, VTubers & their teams) — TypeScript, part of the web platform
- **Scheduling dashboard:** drag-and-drop calendar; AI auto-scheduling suggestions; availability windows (fans/collabs can propose into open slots); tentative schedules with fan voting; cross-platform auto-posting; bulk reminders; time-blocking; workload balancing; burnout tools (energy/availability tracking, automated breaks, response-rate expectation setting).
- Page builder: drag-and-drop blocks + theming; **full-site customization tier**: creator can deeply customize their public site in a **preview/test sandbox**, ships only after an approval submission (platform review). Long-term: full-scale overhauls that can radically distort layout; near-term: simplistic changes only.
- Store manager: tiers (creator defines which features each tier unlocks — chat rooms, exclusive content, VODs, VIP perks are all **per-tier toggles**), one-time goods, merch with fulfillment/shipping, digital products (voice packs, asset packs), collections, drops, PPV events, discounts, newsletters, exportable email list.
- Content management: upload exclusives, batch Q&A responses, poll creation, fan-content approval queue, private VODs per tier.
- **VIP outreach:** direct messages/videos/calls/asset drops to VIP fans for birthdays/holidays; VIP-only chat rooms; custom birthday celebration packages for fans; lottery/randomized access (voice-call lotteries, group AMAs) to scale intimacy without entitlement.
- Community management: channels, roles (incl. fan roles like "Art Mod," "Event Helper"), invites, AI-assisted + human moderation, automated welcomes/rules, announcements.
- Live tools: moderated chat, AMA batching, virtual event hosting with queues/breakout rooms.
- Audience insights: engagement metrics, fan segmenting (active/lurker, timezone, interests), superfan identification for targeted thanks, aggregated (never stalker-ish) views.
- **Transparency & boundaries:** "About" section with stated boundaries ("I don't read DMs — use Q&A"), optional behind-the-scenes/IRL updates with boundary controls, progress trackers ("model rigging update"), reciprocal loops ("what should we do next?" polls, thank-you spotlights).
- Personal productivity: the full Productivity Core (§5.7) is available to solo creators — tasks, habits, Pomodoro, daily planner, ADHD-support tools.
- Collaboration hub: collabs with other creators, shared events, fan-involved projects; single manager interface (§4).
- Brand-kit / theme-pack / monthly-pack uploads (§5.6); analytics; affiliate links; AI assistant (draft posts, summarize feedback, prioritize tasks, content ideas) under §11 rules.
- Partner extras (§7): driver publishing, partner storefronts.
- Optional podcast tools (later): member RSS feeds, episode sync.

### 5.6 Customization Editor + Desktop Agent
**Editor (TypeScript, in Creator Studio):** assemble brand packs and **monthly packs** with live preview and per-platform output. Dashboard designates each asset **mobile-only, desktop-only, or both**. One pack format: zip with manifest.json (name, version, brand, month, platform targets, rarity table, per-resolution assets, checksums).
**Monthly delivery model:**
- Upload path **per month of the year** (all 12); packs auto-deliver on schedule to entitled subscribers and are **saved locally to the user's device on delivery** — owned forever.
- Calendar-aware content: everyone's birthdays, **VIP-tier birthday deliveries**, specific holidays (Christmas, Valentine's), custom date-range holidays (Lunar/Chinese New Year).
- **Rarity system:** multiple assets per slot, categorized common / rare / super-rare with **creator-set % appearance chance**; future Easter-egg pool drawing from a designated rare set.
**Pack contents (creator-customizable):**
- Wallpapers **uploaded per resolution** (desktop sizes; mobile sizes) with one-click apply buttons (desktop + Android; iOS save-and-guide).
- Windows: custom cursors, alert/system sounds (e.g., USB disconnect), **theme colors (taskbar, accent)**, screensavers, widgets.
- App-level (all platforms): custom alarm videos incl. rare-item alarms; live-alert popup videos/animations; alert-symbol customization; **calendar font/color/text theming that propagates into chat**; custom emotes, stickers, GIFs, chat-window backgrounds, profile banners (static + GIF); custom sounds throughout.
- **Per-event custom alerts:** new friend message, artist response, manager response, CRM/kanban board updates — each with its own sound/visual.
- Mobile: ringtones + text tones (Android), widgets, icons, in-app equivalents on iOS.
- Exploratory: **weather-reactive wallpapers** (coarse city/ZIP location → weather API → pack variants change with real-world weather).
**Windows Agent (C#/.NET):** background app + tray icon; device link; downloads packs; verifies signed license + checksums **offline**; snapshots original settings before first change; applies/switches/removes per-brand per-device (registry/SystemParametersInfo; .scr screensavers; Rainmeter-style widget layer; Windhawk-style deep tier behind explicit warning, auto-degrades on Windows updates); installs partner-signed drivers with per-install consent + verification; self-updates; code-signed; antivirus whitelisting submissions expected.
**Mac Agent (Swift):** honest subset — wallpaper, widget layer, in-app sounds (System Integrity Protection forbids the rest; market accordingly).
**Ownership:** assets + signed license live on-device; fully offline; no phone-home DRM.

### 5.7 Business Manager (workspace / CRM / workforce — scales solo → manager → agency → any business) — TypeScript UI on the shared server
**Productivity Core (the merged Todoist/Fantastical/Superlist/SuperProductivity feature set — available to creators, managers, agencies, businesses; modular toggles):**
- Capture: natural-language quick-add (text), widgets, keyboard shortcuts, browser extension, templates.
- Tasks: projects/sections/infinite nesting/subtasks, labels/tags/priorities/custom fields/color-coding, dependencies, recurring tasks, rich notes/checklists/attachments on tasks.
- Views: List, Kanban, Calendar, Timeline, Database, **Eisenhower Matrix**, custom filters; smart Today/Upcoming/Inbox; powerful search; multi-hierarchy organization.
- Calendar: day/week/month + mini calendar + day ticker; unified tasks+calendar (drag to reschedule/timebox); meeting proposals, availability sharing, RSVPs, conference-link handling; **calendar sets** for context switching (work/personal/persona); two-way external calendar sync.
- Focus: built-in time tracking (manual + per-task) with timesheets/exports, timeboxing, Pomodoro, focus mode, break reminders, daily/weekly planning.
- Habits & goals: habit tracker, streaks, heatmaps, progress visualizations, goal tracking; **ADHD/neurodivergent support tools**.
- Collaboration: shared workspaces, assignments, comments/@mentions, guest sharing/public links; docs/wiki (block editor — BlockNote/TipTap-class; Yjs-class sync when collaboration lands); whiteboards (desktop emphasis).
- Unified inbox; offline-first with sync; keyboard-first; themes.
**Manager mode (lightweight, no agency required):**
- Multi-creator oversight dashboard: individual + aggregated calendars, schedules, analytics; task delegation, approvals, project tracking across talents; booking/scheduling links; simplified CRM of talent relationships/opportunities; communication hub (announcements, team chat); fan-insight + community-health metrics per talent; AI-suggested optimizations; edit rights over talent public pages/schedules per permissions.
**Agency tier (enterprise):**
- Talent CRM: contacts, deals, **contracts**, talent profiles; talent onboarding, training modules, document management, performance reporting.
- **Milestones & rewards:** per-talent metric targets (e.g., 100k/1M/2M followers) with reward workflows (the "grant a wish" pattern); **Social-Blade-style stat tracking + projection of when milestones will be hit**.
- Resource allocation: team scheduling, workload views; **studio-time reservation** (recording sessions, Shorts/TikTok batches, birthday lives) on the shared availability engine.
- **Agency events:** company-wide projects (EnReco-style) with mandatory-participation tracking; talent-requested events (e.g., agency-funded Minecraft server) with request→approval workflow; convention-scale events via Event Manager (§5.9).
- Monetization oversight: aggregated memberships, payouts, merch, sponsorship tracking.
- HR/ops: time tracking, employee scheduling, checklists, forms; compliance + moderation oversight across all linked communities; OKRs, wikis, custom databases; automated workflows/rules/bulk actions; SSO, backups, import/export, enterprise security.
**Business features (retail/restaurant/service):**
- Shift scheduling: availability + blackout dates, auto-assignment, open-shift claims, swaps, no-show tracking; team messaging.
- Customer CRM: contacts, pipelines, order history, loyalty tiers; digital membership programs (member pricing, member-only delivery, exclusives); menu/price management (at-market updates, scheduled specials like "Taco Tuesday", effective-dated changes); event-calendar broadcasting to followers; support-desk ticket pipeline (also serves partner hardware support).
- Businesses opt into the same stores/customization as creators.
- Delivery: desktop + web; employee-facing schedule/chat views in the Mobile App.

### 5.8 Register (POS) — TypeScript on tablets + Stripe Terminal
- Customizable cashier layout per business; orders, modifiers, tabs; card readers via Stripe Terminal (no card data touches our code).
- Member recognition via BLE or QR; automatic member discounts; every sale written to customer history for loyalty recognition; offline mode with queued sync.
- **Restaurant QR flow:** table/counter QR → menu view → app-install prompt → on install, auto-link user as **member + follower** of the business, add its calendar, and show a tutorial for filtering it.

### 5.9 Event Manager (venues/conventions/agencies) — TypeScript dashboards on the shared server
- Ticket tiers + online sales; rotating-barcode issuance; front-desk check-in flips entitlement to "confirmed."
- Floor plan: staff uploads 2D image; pin drops per booth; booth-owner edit access; visible **only** to checked-in holders.
- Panel scheduling pushed to attendees; per-attendee choose/block; watch parties/meet-and-greets with limited spots.
- Artist Alley: per-artist QR → buy merch + **auto-follow**. Sponsor booths: QR or BLE handshake → visit confirmed → digital goods granted.
- Vendor/sponsor private Discord-style rooms scoped to the event, with goods distribution.
- **Digital browsing ticket:** paid remote tier — in-venue digital catalog + staff-escorted private livestream tour (LiveKit) with invite-only chat and participant list.
- Agency-scale event support (§5.7); post-event analytics.

---

## 6. Cross-Cutting Systems

### 6.1 Friends & Social (**NON-NEGOTIABLE set**)
- Friends can **share a scheduled event into each other's calendars**; on invite approval, a **group chat** is created for the event with **file sharing** (media, thumbnails); supports "soon-to-be-announced" placeholder dates shared privately between friends before public reveal.
- **Show friends' CURRENT local time** while events are being set up; **additional world clocks** in-app tracking the user's important time zones, displayed as tiny profiles during scheduling.
- **One-click event import via invite link** (exactly like Discord server invites) for recipients who don't have the software — link → landing page → import on signup.
- Fan discovery/friend connections; co-watching; meetups.

### 6.2 Commission & Asset Marketplace (dual mode)
- **Direct mode (VGen-style):** browse artists, request → accept/decline → payment hold → delivery via gated-content system → release. Portfolio pages; creator↔artist relationship tracked in the identity graph.
- **Anonymous stack mode (Skeb-style):** requester posts prompt + reference images + price they'll pay → enters a public stack → any artist claims it (removed from stack) → **delivery time limit; auto-restacked if missed** → delivery → payout. **No direct communication** between parties; anonymized both ways. Used for emotes, thumbnails, promo assets.
- Custom alert when an artist responds/delivers (§5.6).

### 6.3 Anonymous Q&A (Marshmallow-style)
- Fans submit questions anonymously; **automatic insult/toxicity filtering** to keep it a safe space; creator sees a filtered list and chooses what to answer.
- Creator does not see the asker; **the fan is notified when their question is answered**; account creation underpins block/protection and abuse handling; **optional mutual identity reveal** only if both sides opt in.
- Batched answering tools in Creator Studio; custom alert on answer (§5.6).

### 6.4 Ingestion, Notifications & Media (unchanged from v1.0, summarized)
- Twitch EventSub push; YouTube WebSub + quota-aware polling; everything normalizes to one activity/event record. Social: creator-connected accounts only — Bluesky/Mastodon first; X (paid API) and Meta (Graph API + review) post-revenue. **Never scrape.**
- Activity → follower fan-out queue → SignalR (open app: branded animated toast from cached brand kit) or OS push (closed app: plain text+image; OS notifications cannot play custom animations). Brand kits versioned + CDN-cached per followed creator.
- Media pipeline: upload once → ffmpeg → WebM VP9-alpha (Chromium) + HEVC-alpha (Apple/WebKit); HLS for video; caps (stingers ≤ 3 s, ≤ 10 MB). Overlay support order: alpha video → Lottie (theme-recolorable) → GIF/APNG/WebP → sandboxed HTML/CSS widget packages (later, security-reviewed, iframe-sandboxed).
- Gated content: object storage + short-lived signed URLs after entitlement check.
- Chat & voice: SignalR text/presence; LiveKit voice/video/screen/streams. Discord *parity* is a long-term direction built incrementally; membership↔chat-role linkage is the near-term value.
- Offline-first: tickets, floor plans, Register, owned packs, monthly deliveries, gated downloads all function offline and sync later via the signed-license pattern.

### 6.5 AI Assistant Layer (all roles; §11 rules apply)
Smart scheduling suggestions, content summarization, post drafting, task prioritization, personalized fan recommendations ("fans like you enjoyed this clip"), moderation assist, agency optimization suggestions. All outputs are proposals validated against real permissions.

### 6.6 Local & Ambient Discovery (exploratory, post-revenue)
- **Geo business discovery:** ZIP/city-level lookup of "Live Event Calendars" near the user, interest-filtered (Reddit-city-forum-style weekly activity view).
- **Weather-reactive customization** (§5.6). Both use coarse location only, opt-in.

### 6.7 Accessibility & Inclusivity
Multi-language, captioning on streams/VODs, customizable interfaces (fonts/sizes/contrast — dovetails with creator theming), keyboard-first desktop, screen-reader support targets.

---

## 7. Partner Program (Hyte / Starforge / MetaPCs-class) — unchanged from v1.0
1. Partner storefront: themes for desktop/tablet/mobile/web, limited-time offers, news, schedules, streams — partner opts into any subset.
2. Affiliate/outbound links from creator pages to partner product pages (sponsorship tie-ins).
3. Full OS brand packages sold as products (the "beyond the PC case" pitch).
4. **Driver/software distribution:** partner uploads their *already-signed* installer once (kernel drivers WHQL-certified under the PARTNER'S certificate — we are the delivery truck, never the signer). Server records checksum; opted-in users' Agents verify signature + checksum and install only after per-install consent. Mandatory: partner 2FA, audit log, Agent-side verification. This is a software supply chain — its security is existential.
5. Partner support desk on the shared support-chat + CRM pipeline.

---

## 8. Platform Limitations (set expectations in product & marketing)

### 8.1 Browsers/engines
Chromium + Firefox: WebM-alpha. Safari/WebKit: HEVC-alpha (handled by transcode pipeline). On iOS every browser is WebKit; Mac desktop app ships Electron for Chromium uniformity.

### 8.2 OS customization ceilings
- **Windows:** ~80–90%. Safe tier (wallpaper/sounds/cursors/screensavers/widgets/accent+taskbar colors) fully supported; deep tier (start menu, interface mods) best-effort, can break on Windows updates, auto-degrades.
- **macOS:** ~25%. Wallpaper, widgets, in-app sounds only (System Integrity Protection).
- **iOS:** ~10%. Widgets + our-app icons; wallpapers save-and-guide; sounds in-app only; **no custom ringtones/text tones programmatically**.
- **Android:** ~40%. Programmatic wallpapers/live wallpapers, widgets, icon packs, **ringtones + notification tones settable with user permission**. No system-wide theming.
- **Web:** themes our own app only.

### 8.3 External-platform risk
YouTube quota (10k units/day default) — cache aggressively, push where possible, apply for increase once live. Twitch/YouTube ToS goodwill is the largest external risk; membership features adjacent to their monetization need policy tracking. X/Meta ingestion is paid/reviewed — not early scope.

---

## 9. Functional References (reference or federate — do not transpile)

| Domain | Reference | Note |
|---|---|---|
| Booking/availability | Cal.com | Hardest-to-rebuild logic (availability, time zones/DST, calendar sync). **AGPLv3** — design reference or unmodified self-host only. |
| CRM | Twenty (benchmark: Attio) | Rebuild needed subset natively. |
| Chat/servers | Revolt / Discord | Patterns only; transport is SignalR. |
| Docs/blocks | AFFiNE / AppFlowy; BlockNote, TipTap, Lexical; Yjs | Web editor ecosystem is the build path. |
| Tasks/productivity | **Super Productivity (open source)**, Todoist, TickTick, Superlist, Fantastical | Feature/UX references for the Productivity Core (§5.7). |
| Agency/ops | Monday.com, Paylocity, ClickUp, Connecteam | Workflow + HR/ops references. |
| Page builder | Puck | Embeddable React visual builder. |
| Ticketing | pretix | Event Manager reference. |
| Notifications | Novu | Reference; ours is custom on SignalR + queues. |
| Voice/streams | LiveKit | Used directly (self-hosted). |
| Streamer calendar UX | Holodex (open-source frontend) | Live grid/multiview reference. |
| Memberships/publishing | Patreon | Feature reference distilled into §5.5 (tiers, one-time, merch fulfillment, collections, drops, newsletters, discounts, discovery, team seats). |
| Creator pages | Carrd | One-page builder + free/pro tier model reference for our page tiers. |
| Commissions | VGen, Skeb, Pixiv | Dual-mode marketplace (§6.2) + art-community UX. |
| Anonymous Q&A | Marshmallow-QA | Safety-filtered anonymous Q&A (§6.3). |
| Stats/projections | Social Blade | Milestone projection reference (§5.7). |
| Desktop widgets | Rainmeter | Windows widget/skin engine blueprint; can be driven directly at v1. **GPL-family** — drive unmodified engines; publishing required if we ship modified engine code. |
| Windows deep mods | Windhawk | Same licensing note; deep-tier blueprint + mod-marketplace model. |

---

## 10. Monetization

- Fan-facing apps: **free**.
- Digital sales & memberships: **15% + processing** (Stripe passed through). Creator-defined tiers with per-tier feature toggles; free tier supported.
- Physical merch: **5–8%** (or 15% of margin).
- Tickets: **~2% + $0.99/ticket**.
- Business Manager: **per-location monthly SaaS** (~$59 reference); **free base tier for small teams, price scales with employee count/features**.
- **Storage subscriptions:** monthly cloud-storage tiers for agencies/creators with large archives (the "huge database" revenue line).
- Register: free software; monetized via membership commerce + processing.
- Partner program: 15% on partner theme sales + affiliate commissions + flat partner tier for driver-distribution/support pipeline.
- Monthly packs: sold as membership-tier benefits or standalone purchases; **sellable digital calendar** is a first-class product.
- Optional Creator Pro: reduces take (15%→10%), advanced analytics/pack slots.
- All digital checkout on **web** (never Apple IAP). Bandwidth watch-item: meter/cap video by tier from day one.

---

## 11. Security Requirements (non-negotiable; in the first scaffold)

Baseline (OWASP ASVS Level 2 checklist): Argon2 hashing; **mandatory 2FA** for creator/partner/venue/manager/agency/admin; OAuth tokens encrypted at rest; least-privilege DB roles; secrets vaulted; authorization checks on every route with default-on tests; WAF + bot challenge, per-IP/per-account rate limits, email verification, anomaly alerts; CI gates (Semgrep SAST, dependency/container/secret scanning); audit logs on money, entitlements, admin, partner uploads, manager edits to talent property; signed licenses + checksums for packs; full signature/checksum/consent chain for drivers (§7.4).

Content safety: moderation pipeline for fan uploads and community content (report/ban tooling, automated pre-screening incl. CSAM scanning for user media); anonymous Q&A safety (§6.3 filtering, block/protection, abuse reporting with accountable accounts behind anonymity).

AI-specific: (1) human line-review by the architect on every AI-written line touching auth, payments, tokens, entitlements, private content (CODEOWNERS-enforced); (2) prompt injection — LLM features output *proposed structured actions* validated server-side against caller permissions; model output is untrusted; (3) anti-automation — assume AI-accelerated stuffing/scraping/bulk signups from day one.

**Frontend cross-references:** `USER-STORY-account-security-settings.md` speces the online-device list + remote-logout UI against this section's session/auth baseline (no new backend behavior, just surfacing it). `USER-STORY-audit-log.md`'s server-backed history retention is the calendar-specific instance of this section's audit-log requirement for manager edits to talent property.

---

## 12. MVP Scope & Roadmap

**MVP loop (UNCHANGED by v1.1):** connect Twitch/YouTube → aggregated live/scheduled calendar → follow creators → notifications → creator page → paid membership (Stripe) → gated content (signed URLs). Web-first; PWA install at beta.

- **Phase 0 (wk 1):** register blockers — Twitch dev app, Google/YouTube API project, Stripe Connect, Apple/Google accounts. Scaffold monorepo, CI, auth, security baseline (§11).
- **Phase 1 (wk 2–3):** ingestion workers, events table, calendar UI, follows.
- **Phase 2 (wk 3–4):** creator pages (template + theme colors), notification fan-out (web push + email).
- **Phase 3 (wk 5–6):** Stripe subscriptions, entitlements ledger, gated content.
- **Phase 4 (wk 7+):** closed beta with 5–10 real streamers; hardening.

**Post-MVP order (v1.1-updated; NON-NEGOTIABLES land early):**
1. Text chat (SignalR) + **friends & event sharing set (§6.1: shared events, group chats + files, friend current-time + world clocks, invite links)**.
2. Polls, Q&A submission, **anonymous Q&A (§6.3)**, holiday layers + toggles.
3. Brand packs + Windows Agent safe tier + **monthly pack delivery, rarity system, one-click wallpaper apply, per-event custom alerts, VIP birthday deliveries**.
4. Commissions (direct mode → anonymous stack mode) + marketplace.
5. Productivity Core → Manager mode → Business features → Register.
6. Event Manager → Agency tier (milestones/projections, studio booking, agency events).
7. Voice (LiveKit) → deep-theme tier → Mac/mobile agents → partner driver pipeline.
8. Social ingestion expansion (Bluesky/Mastodon → X/Meta) → gamification depth → wellness suite → discovery/weather (§6.6) → full-site overhaul customization (approval + sandbox) → Discord-parity long tail → podcast tools.

**Engineering conventions:** trunk-based; every AI change is a PR; Dev 2 authors feature PRs against written specs; architect owns /auth, /payments, /api via CODEOWNERS; CI blocks merges without tests; OpenAPI contracts published first, frontend builds against mocks.

**Agent acceptance tests:** buy → apply → airplane mode → reboot → pack persists/switches; uninstall restores original desktop exactly; tampered pack/forged license refused; bad driver checksum/signature refused; Windows update with deep mods degrades to safe tier; monthly delivery saves locally and remains after subscription lapse (owned forever).

---

## 13. First Claude Code Prompts

1. *"Here is MASTER_SPEC.md v1.1. Produce PRODUCT_SPEC.md and ARCHITECTURE.md for the MVP only (§12 loop): ASP.NET Core 9 + SQL Server + SignalR backend, React (Vite)/TypeScript frontend, monorepo layout, entity model (users, profiles/personas, friends, creators, follows, events, entitlements, subscriptions), OpenAPI draft. Also INFRA_OPTIONS.md comparing managed vs self-hosted per component (database, API host, object storage, queue, email/push) with monthly cost at 0–1k and 10k users; recommendation column left open for the architect. Design the schema so §4 roles and §6.1 friends/event-sharing bolt on WITHOUT migration pain, but implement MVP scope only. Flag every open decision."*
2. *"Generate SECURITY.md from §11: threat model (including AI-accelerated attackers and prompt injection), OWASP ASVS L2 checklist mapped to the stack, CI security gates, content-safety pipeline outline."*
3. *"Scaffold the monorepo per ARCHITECTURE.md: docker-compose (SQL Server/Redis), ASP.NET Core with Identity + 2FA + rate limiting, React (Vite) with auth pages, CI running tests + security gates. No feature code yet."*
4. One Phase-1 feature per prompt thereafter (e.g., *"Implement Twitch EventSub subscription handling per spec, with tests"*). Never "build the app."
