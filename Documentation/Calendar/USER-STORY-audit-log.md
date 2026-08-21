# User Story — Folder & Calendar Audit Log

**Feature name:** Audit Log
**Status:** Draft
**Related:** [USER-STORY-nested-folders-calendar.md](USER-STORY-nested-folders-calendar.md), [USER-STORY-live-edit-pulse.md](USER-STORY-live-edit-pulse.md) (same change event triggers both), [USER-STORY-folder-lock-settings.md](USER-STORY-folder-lock-settings.md) (shares the Settings card for history size), [FrontEndLayout.md](FrontEndLayout.md) (on-screen entry point)

---

## Story

**As a** creator who delegates calendar edits to managers/staff,
**I want** a reviewable history of who changed what and when — filterable to just manager-made changes and sorted by urgency — with control over how much history is kept,
**so that** I can catch mistakes, understand why my schedule looks different than I left it, and audit manager activity without digging through chat logs.

---

## Layout

- An on-screen **audit log button** near the folder sidebar's header (in `FrontEndLayout.md`'s left panel) opens the log as a panel/overlay.
- Log entries, newest first, each showing: **timestamp**, **actor** (who made the change), **folder/item affected**, and **change type** (created, moved, renamed, locked/unlocked, visibility toggled, deleted, recolored, etc.).
- A view filter: **All changes** vs. **Manager changes only** — the manager-only view supports sorting by **time-sensitivity** (how urgent the affected event is) rather than strictly by when the change happened, so a creator reviewing manager activity can prioritize "did they touch something that matters soon" over raw recency.

## Behavior

- Every folder/event mutation (per `USER-STORY-nested-folders-calendar.md`'s behaviors, plus lock/color changes from the other story files) writes one audit-log entry, regardless of who made it.
- **Retention:** the log keeps the most recent **50 entries by default**; a Settings control (shared card with `USER-STORY-folder-lock-settings.md`) allows scaling retention up to a **maximum of 500**. Once the cap is reached, the oldest entry is dropped as each new one is recorded (ring-buffer behavior, not silent data loss beyond the configured cap — the cap is the intended limit, not a bug).
- History is also backed up server-side (not just client-local), so a user switching devices sees the same log — this rides on the same account/sync backbone as the rest of the app (MASTER_SPEC §5.1), not a separate local-only store.
- Double-clicking/selecting a log entry can jump to the affected folder or event in the calendar view (not a hard requirement for v1, but the data model should support it — each entry needs a stable reference to the folder/event it describes).

## Settings

| Row | Control |
|---|---|
| History size | Stepper, 50 (default) – 500 |

(This row lives in the same Settings card as the lock/manager-override controls in `USER-STORY-folder-lock-settings.md` — one "Folder Settings" card covers both who-can-touch-what and how-much-do-I-remember-about-it.)

---

## Acceptance criteria

- [ ] Every folder/event mutation — by the owner or by a manager/staff member — produces exactly one audit-log entry with timestamp, actor, affected item, and change type.
- [ ] The audit log button is visible near the folder sidebar and opens the log without leaving the current calendar view.
- [ ] Default retention is 50 entries; entries beyond the configured cap are dropped oldest-first.
- [ ] Raising the History size setting up to 500 retains more entries going forward; lowering it does not retroactively delete already-kept entries below the new cap in a way that surprises the user (define exact behavior at build time — e.g., trim on next write vs. immediately).
- [ ] The "Manager changes only" filter shows exclusively manager/staff-originated entries.
- [ ] Manager-only view supports sorting by time-sensitivity of the affected event, not just by change recency.
- [ ] Log data persists across devices for the same account (server-backed, not local-only).

## Out of scope
- Undo/revert directly from the audit log (viewing history only, not reverting it) — noted as a future enhancement, same as `USER-STORY-nested-folders-calendar.md`'s existing "undo" out-of-scope note.
- Exporting the log (CSV/PDF) — not requested, defer until asked for.
