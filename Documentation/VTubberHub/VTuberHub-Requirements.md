# VTuberHub — Creator Website Requirements

Goal: Add a creator website.

Implementation: On the calendar page, if a user is a Creator, enable the VTuberHub button in the lower-right corner.

Requirements: When the Creator clicks the button it redirects them to their branded website. If no creator website exists, ask for one to be generated.

A sub-button, "Edit Public Website", sits alongside it. It opens the site in an editable preview where the Creator (or a permitted manager/editor) can customize and edit before publishing the final product. Nothing reaches the public site until it is explicitly published.

Use Creator Hub (the existing build, being renamed VTuberHub) as the baseline of features that need to be filled in on the generated website.

---

## PART 1 — Creator Hub feature baseline

Architecture today: static HTML/JS pages, one file per tool, sharing a Supabase backend (Postgres + Row Level Security + Storage + Auth). Sign-in is magic link (email, no passwords) with "Continue with Google" as a fallback. Most tools work signed-out; sign-in adds persistence and sharing. All ML runs on-device in the browser — nothing is uploaded for analysis.

### 1. Content planner (planner.html) — the core workspace
- Three views of the same content: Kanban (7 status columns, drag-and-drop ordering), List (title / status / platforms / assignee / deadline), Calendar (month grid with deadline, posted, and recurring stream-slot markers).
- Item editor per content piece: title, priority star, status, assigned editor, platform tags (YT Shorts, YT Long, TikTok, Instagram, X, Twitch), deadline and posted date/time, hook, script, hashtags, thumbnail URL, footage URL, editor-output URL, notes.
- File attachments per item (up to 5 GB each) into per-owner/per-item cloud storage, plus unlimited labeled "additional asset" links.
- Inline title scoring — scores the title as you type, with a jump into the full Optimizer.
- Comment threads on every item, tagged by role (owner / manager / editor).
- Teleprompter / recording mode — fullscreen script reader, keyboard-driven (play-pause, restart, speed, font size, mirror for camera).
- Brand kit — branding images, editing-style reference videos, asset-folder links, and creator notes; visible to every assigned editor.
- Stream schedule — 7-day recurring slot grid with times, titles, and colors.
- Embedded to-do widget with quick-add and streak chip.
- Live sync — changes appear across devices and collaborators in real time.
- Toolbar filters: by editor, by form (short / long / important), free-text search.

### 2. Team & delegation
- Editor roster — add editors by name/email/color, generate a personalized invite link, preview exactly what an editor sees, revoke access.
- Editor dashboard (planner-editor.html) — editors see only work assigned to their email, grouped Ready / In progress / Done. They can read the brief, grab footage, upload the cut, leave notes, and move items through recording → editing → scheduled. Publishing stays creator-only. Brand kit always visible.
- Manager delegation — full, revocable co-owner access to the whole hub. Invites can be locked to one email or left open for anyone with the link. A persistent banner shows whose hub a manager is operating in.
- Invite claim flow (manager-claim.html) — link → sign in → claim → land in the hub, with auto-claim when already signed in.
- Manager dashboard (manager-hub.html) — every creator who has delegated to you, with per-client status counts, "needs attention" flags (scheduled this week / stuck in editing >5 days / new editor notes), pinning, display-name overrides, and private per-client notes.

### 3. Task management (todo.html)
- Board (Today / This Week / Later / Done), List, and Today-only views.
- Quick-add with inline shortcuts (#tag, !today/!week/!later, trailing ! for priority).
- Task editor: bucket, category, priority, due date/time, time estimate, notes, subtasks, tags, comments.
- Recurring tasks — daily, weekdays, weekly, monthly, custom; a fresh task spawns on completion.
- Linking — attach a task to a planner content item via a search picker.
- Focus timer — pomodoro with pause/skip, a floating pop-out picture-in-picture window, and logged sessions feeding a streak counter.
- End-of-day review — stats, weekly chart, completed list, and "roll everything forward to tomorrow".
- Custom categories with icons and colors.

### 4. Content Optimizer (analyzer.html)
- Upload a short → on-device ML pipeline (vision captioning + OCR, speech transcription, semantic niche matching, audio scene classification, face detection) → score out of 100 with grade and verdict.
- Criteria breakdown sorted by score impact, plus a fix checklist.
- Rewrites and suggestions — title rewrites, hashtag picks using a 1-short/2-medium/2-large formula.
- Post pack — ready-to-paste YouTube title + description, TikTok caption, Instagram caption, and a 280-character tweet.
- Also generates titles, tags and descriptions for streams and long-form, not just shorts.
- Content-safety flags surfaced at the top of results.
- Creator profile — display name, niche, VTuber style, content forms, voice/tone, audience, platforms, goals, candidate tag library, topic synonyms — biases every suggestion toward that creator.
- Learning loop — thumbs up/down on titles and tags reweights future recommendations for that user.
- Trend bias — suggestions weighted toward currently-rising keywords from a periodically refreshed trends cache.
- Saved run history, sticky tags, description-template presets, and posted-performance logging (URL, views, likes, comments, shares, notes).
- Knows ~100 games, 39 genres/topics, 35 content types, and 7 creator-identity tags.

### 5. Thumbnail checker (thumbnail.html)
- Drop a thumbnail → score out of 10 with verdict and criteria grid.
- Multi-size preview across every real YouTube/Twitch mobile and desktop display size.
- Extracts what it sees: on-image text (OCR, editable), faces, focal point, contrast, colors — with a toggleable detection overlay.
- Character checklist built for 2D VTuber models (visible / emotion / big crop / eyes to camera), because face auto-detection is unreliable on VTuber art. Score updates live as it is ticked.
- Fix list plus a best-practices reference panel.

### 6. Growth playbook (growth.html)
Long-form reference behind everything the Optimizer scores — 18 chapters, collapsible, with a jump-link index: algorithm mechanics, the 3 metrics that matter, the 4-criteria content rule, YouTube Shorts specifics, driving comments, platform-specific posting rules, the hashtag formula, growing on Twitch, title strategy, chat engagement, conversion and retention, monetization and sponsors, Twitter/X for VTubers, a Twitch deep-dive, OBS setup for quality, VTuber monetization in depth, Fansly (18+), and a resource list.

### 7. Sustainable habits (habits.html)
- Spoon-theory energy model: pick Low / Medium / High for the day and only habits that fit your energy appear.
- Streaks pause instead of resetting — a missed day does not wipe progress.
- Stream-day toggle reveals pre-stream / on-air / post-stream habit sets.
- Curated creator habit library across 8 categories, plus custom habits with emoji, energy cost, and tracking type (check-off / weekly target / count with units).
- 30-day heatmap and stats. Works offline-first locally; sign-in syncs across devices.
- Each habit category links back to the relevant growth-playbook chapter.

### 8. Sponsor and monetization suite
- Media kit (media-kit.html) — a public, shareable, sponsor-facing one-pager plus its own editor. Identity and branding (name, tagline, bio, avatar, banner, pronouns, location, languages), niche and vibe tags, content pillars, per-platform channels and stats with paste-a-URL handle extraction, audience demographics (age brackets, gender split, countries, interests) rendered as charts, standout content, past sponsorships and testimonials, services and rate card with per-item hide toggles, and contact/booking CTA. Custom vanity slug, private/public toggle, and a print stylesheet so it exports as a clean sponsor-ready PDF.
- Sponsor pitch builder (sponsor-pitch.html) — a 5-step wizard (brand → sponsorship type → offer and deliverables → tone of voice → send) that generates a cold email, Twitter DM, Discord DM, Instagram DM, a printable one-page pitch doc, and a rate-card snapshot, all pulled from the media kit and all fully editable, with per-channel character counters. 8 sponsorship types supported. Pitches are pipeline-tracked (draft → sent → responded → signed / passed) with outcome notes, and each pitch stores the rates that were actually quoted.
- Sponsor rate calculator (rate-calculator.html) — enter audience size and views, get a transparent, research-backed rate card that drops straight into the media kit and pitches.

### 9. Onboarding, planning and wellbeing tools
- Niche Finder (niche-quiz.html) — two-part quiz finding both your content niche and your stream vibe.
- Schedule Shaper (schedule-quiz.html) — builds a content schedule around real energy and life, grounded in pacing and burnout research, with explicit accommodation for ADHD, chronic illness, disability, and a full IRL plate.
- Debut checklist (debut-checklist.html) — every commission, asset and setup task needed before debut day, with costs, artist links, payment tracking, and a countdown.
- Subathon planner (subathon.html) — goals, incentives, timer rules, and a day-by-day schedule for a subathon/donothon/marathon, plus a live countdown timer widget (subathon-timer.html) designed to be dropped into OBS as a browser source.
- Creator check-in (review.html) — gentle weekly review: wins, content, community, mental check, and the one goal that matters next week.
- Creator Memory (brand-memory.html) — one persistent profile of who the creator is (voice, audience, series, word bank, facts) that every other tool reads from so output stays on-brand rather than generic.
- Ask My Planner (ask.html) — natural-language query over your own planner ("what should I film this week?") returning ranked cards, run in-browser.
- Competitor Scout (scout.html) — weekly view of VTuber channels worth learning from: big benchmarks, fast-growing breakouts, and which Shorts formats are currently working.
- The Toolbox (toolbox.html) — small rule-based helpers: break a task into steps, shift the tone of a message, sanity-check how a message reads, and get an honest time estimate.

### 10. Site-wide / platform features
- Homepage (index.html) — brand hero, tool tile grid, personalized welcome card, habits summary widget.
- Auth — magic link plus Google sign-in, one session shared across every tool, account menu, and self-service account deletion that cascades all data.
- Accessibility modes (a11y-modes.js) — site-wide display/accessibility toggles.
- Demo mode — append ?demo=1 to get a fully sandboxed fake-signed-in session backed by local storage, touching no real data. Used for safe testing and for showing the product without an account.
- Installable PWA — web manifest and service worker; installs to phone/desktop and works offline where the tool allows.
- Changelog (changelog.html), FAQ (faq.html), About (about.html), Contact / bug report (contact.html).
- Security model — Postgres row-level security on every table, delegated access through audited stored procedures rather than direct writes, signed time-limited URLs for private files, and hidden rate-card pricing stripped server-side before public rendering.
- Shared design system — one CSS token set, shared nav, shared modal library, consistent typography and branding across all pages.

---

## PART 2 — Additional requirements for the generated creator website

These are the gaps between "Creator Hub as an internal toolset" and "every VTuber/streamer gets their own branded public website."

### A. Multi-tenant and identity
1. Every Creator gets an isolated tenant. Today's build assumes one creator; the generated site must scope all data, storage, and slugs per creator with no cross-tenant read path.
2. Site address — the platform owns the domain. Each creator gets a vanity path on it, Patreon-style (vtuberhub.com/creator), with a reserved-word list, collision handling, and rename support with redirects from the old address. Creator-owned custom domains are out of scope for this build.
3. Creator role flag on the user account, since the calendar page must decide whether to show the VTuberHub button.
4. Provisioning flow — the "no site exists yet" path: prompt, confirm, generate, and land the creator in their new site. Must be idempotent and safe to retry.
5. Permissions — the Creator holds Admin on their own site: they grant, scope, and revoke access. Managers and editors receive User-level edit rights, set per-person by the Creator. User level can edit and save drafts; publishing to the public site is Admin-only unless the Creator explicitly grants it. Every edit records who made it, and the Creator can revoke access at any time with immediate effect.

### B. Site generation
6. Template/theme system — pick a layout, then override colors, fonts, banner, avatar, logo, and mascot. Must ship sensible defaults so a site looks finished the moment it is generated with zero configuration.
7. Seed the site from what is already known — pull the creator's existing profile, brand kit, media kit, and Creator Memory into the new site rather than presenting an empty form.
8. Page/section builder — enable/disable and reorder public sections: about, schedule, content, socials, sponsors/media kit, shop/merch, support/donate, contact, FAQ.
9. Live preview and publish states — draft vs published, preview link, and a rollback to the previous published version.

### C. Public-facing pages the site must ship with
10. Landing/about page with bio, pronouns, languages, lore, and model/artist credits.
11. Stream schedule rendered publicly, in the visitor's local timezone, sourced from the existing planner stream-slot grid.
12. Content showcase — latest videos/clips/VODs, ideally auto-pulled from connected platforms rather than manually entered.
13. Links hub — a link-in-bio replacement covering every platform, so the creator can drop VTuberHub as their single social link.
14. Public media kit at a clean URL, with the existing private/public and hidden-pricing controls preserved.
15. Support / monetization block — donations, memberships, merch, wishlist, affiliate links.
16. Contact / business inquiry form routed to the creator's business email, with spam protection.
17. Community — Discord invite, rules/boundaries page, FAQ.
18. Press kit / VTuber assets — downloadable logos, model reference, emotes, and usage terms for collab partners and clippers.

### D. Integrations
19. Platform connections — YouTube, Twitch, TikTok, Instagram, X, Discord, Kick. Needed for auto-pulled content, live status, and real stats.
20. Live now indicator — the public site should show when the creator is live and surface the embed/link.
21. Auto-filled stats — media-kit stats are manual today; connected platforms should refresh follower/view counts automatically, with a manual override retained.
22. Calendar sync — the calendar page that hosts the VTuberHub button and the public schedule must be the same source of truth, including one-off events and cancellations, not just recurring weekly slots.
23. Payments — a processor for donations, memberships, and merch, with payout setup and tax reporting.

### E. Non-functional requirements
24. Mobile-first — most VTuber audience traffic is mobile; the public site must be fast on phones.
25. SEO and social embeds — per-creator meta tags, Open Graph and Twitter cards, sitemap, and structured data so shared links preview correctly.
26. Performance budget — the public site must not carry the multi-hundred-megabyte ML model downloads used by the Optimizer and thumbnail checker; those stay in the signed-in tool area.
27. Accessibility — WCAG AA on all public pages, carrying forward the existing accessibility modes.
28. Analytics — privacy-respecting visitor analytics the creator can actually read, plus link-click tracking for the links hub.
29. Moderation and safety — content policy for generated public sites, an abuse-report path, age-gating for 18+ creators, and takedown/suspension tooling.
30. Legal — per-site privacy policy and terms, cookie consent, GDPR/CCPA data export and deletion, extending the existing self-service account deletion to cover the public site.
31. Uptime and backups — public sites are the creator's front door; define an SLA, backups, and a restore path.
32. Internationalization — at minimum timezone and date-format correctness; ideally multi-language, since EN VTubers have heavily international audiences.
33. Migration — the rename from Creator Hub to VTuberHub needs redirects from existing URLs, updated branding across all 25+ pages, and preserved existing user data and media-kit slugs.

### F. Decisions already made

Rendering. One shared render for all users who are granted access — not a per-visitor variant. The site is generated from the creator's data and served the same way to everyone with access. Practically this means a published snapshot, with the small number of genuinely live elements (live-now status, current stats) refreshed independently rather than requiring a republish.

Domain. The platform owns the domain. The public site mimics the Patreon model: every creator lives at a vanity path on the platform domain. Creator-owned custom domains are out of scope.

Publishing. Self-serve, not reviewed. There is a preview state where the site can be customized and edited before publishing, reached from the "Edit Public Website" sub-button. Publishing is an explicit action.

Permissions. Managers and editors can edit the public site. The Creator decides who and grants it. Creator = Admin; managers and editors = User level. See requirement 5.

Tiering. Currently everything is free to the Creator. A suggested split is below.

---

## G. Suggested free vs paid split

The principle: anything that costs per-creator money to run should sit behind the paid tier; anything that runs on the creator's own device should stay free. Most of the existing hub costs nothing per user because the ML runs in the browser — that is what makes a generous free tier possible here.

Free — keep as-is
- The entire existing toolset: planner, tasks, Optimizer, thumbnail checker, growth playbook, habits, and every quiz/checklist/wellbeing tool. These run on-device and cost effectively nothing per creator.
- Media kit and sponsor pitch builder, including the public media-kit link.
- One public website on a platform vanity path, with the default themes.
- Core public sections: about, schedule, content showcase, links hub, contact.
- Basic visitor counts.
- One or two collaborator seats (editor or manager).

Paid — where the real cost sits
- Payments and monetization — donations, memberships, merch. This carries processor fees, payout infrastructure, fraud exposure and tax reporting. Either a subscription tier or a platform cut of transactions; the Patreon model argues for a percentage.
- Platform integrations and auto-refreshed stats — live-now status, auto-pulled content, follower/view sync. These are recurring API and polling costs that scale per creator.
- Storage above a free quota — the planner allows 5 GB per attachment today; that is not sustainable as an unlimited free offer.
- Full theme customization — custom CSS, premium templates, removing platform branding from the public site.
- Unlimited collaborator seats beyond the free allowance.
- Advanced analytics — audience breakdowns, referral sources, link-click tracking, historical retention.
- Priority support and higher publish/version-history limits.

Deliberately not paywalled. Do not gate accessibility features, data export, or account deletion. And do not gate the growth playbook or the wellbeing tools — the burnout, pacing and check-in tools are the ones creators most need when they can least afford to pay, and gating them undercuts the product's stated premise.

Open item: whether to charge a subscription, take a percentage of creator earnings, or both. The Patreon comparison implies a percentage; a percentage also keeps the free tier fully featured for creators who are not yet earning.

---

## PART 3 — Implementation status

### H. Part 2 foundation — shipped ([PR #16](https://github.com/fishinpox/Brian/pull/16))

The tenancy/provisioning/permissions substrate from Part 2 (groups A's requirements 1, 3, 4, 5, plus the calendar-page button and requirement 2's vanity-path routing mechanics) has been built. Groups B/C/D/E (theming, page/section builder, public page content, platform integrations, non-functional hardening) are **not** started — the generated site currently has no real content, just a slug and a publish/draft state.

**Decisions made during this pass, resolving/refining open questions above:**

- **Solution placement.** Built into the existing `Solutions/CreatorSolution` (`Creator.{Domain,Application,Infrastructure,API}`, port `:7003`) rather than a new solution — that scaffold already had `/api/creators` and `/api/memberships` Gateway routes and the `UserRole.Creator` enum value already existed in Identity, both clearly aimed at this exact concept. `Shared.Contracts`' existing `MembershipActivatedEvent`/`MembershipCancelledEvent` look aimed at a future *fan* membership/monetization feature (Part 2 group D/paid tier) and are unrelated to the manager/editor site-access grants built here — left untouched.
- **Auth.** Brian's own Identity service/JWT is the single source of truth for Creator identity — no Supabase, no bridging. This also settles Part 1: whenever it's ported, it will run on the same Identity/JWT model rather than keeping Supabase auth.
- **Part 1 (Creator Hub toolset — planner, tasks, Optimizer, thumbnail checker, growth playbook, habits, sponsor suite, etc.) is a separate, not-yet-started effort.** It is a full port into a .NET service (not an iframe/embed of the existing Supabase app), most likely `CreatorSolution` again given the placement decision above, but that's not finalized. Nothing from Part 1 has been built.
- **Seeding (requirement 7) is stubbed for this pass** — there is no Part 1 data yet to pull from, so a newly provisioned site starts empty. The creator picks their own address; nothing is pre-filled beyond a slug suggestion derived from their username.
- **Frontend lives in the existing Calendar.Web app**, not a standalone site-builder app — new routes (`/vtuberhub`) alongside the calendar, not a new deployable.
- **Vanity-path resolution (requirement 2)** uses the same Gateway/YARP pattern as every other service: a low-priority catch-all route (`/{slug}`, `Order: 100`) forwards to `creator-cluster`, resolved server-side by a placeholder `PublicSiteController` — proves the routing plumbing without building real page rendering (that's group C/B, still open).
- **Permissions model (requirement 5)** implemented as `Site` (owner, slug, publish state) + `SiteAccessGrant` (per-site `Admin`/`User` level, revocable via a nullable `RevokedAt` rather than hard delete, so history is preserved). Creator is auto-granted `Admin` at provisioning. Grant/revoke endpoints exist; there's no manager/editor invite-link UI yet (that's still open, along with the rest of group A's invite-flow spirit borrowed from Part 1's "Team & delegation").
- **Provisioning (requirement 4)** is idempotent — re-calling provision for a profile that already owns a site just returns the existing one rather than erroring.

**Still open / not started:** everything in groups B (theme system, page/section builder, real publish/rollback versioning beyond a boolean state), C (every actual public page), D (platform integrations, live-now, auto-stats, calendar sync, payments), E (mobile perf, SEO, accessibility audit, analytics, moderation, legal, i18n, the Creator Hub → VTuberHub rename/migration itself), and the free/paid tiering in section G.
