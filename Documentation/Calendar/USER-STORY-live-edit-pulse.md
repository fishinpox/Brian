# User Story — Live-Edit Pulse Indicator

**Feature name:** Live-Edit Pulse
**Status:** Draft
**Related:** [USER-STORY-nested-folders-calendar.md](USER-STORY-nested-folders-calendar.md), [USER-STORY-audit-log.md](USER-STORY-audit-log.md) (the pulse and the audit-log entry fire from the same change event), [USER-STORY-folder-lock-settings.md](USER-STORY-folder-lock-settings.md) (manager-override edits also pulse)

---

## Story

**As a** creator whose managers/staff can edit my calendar,
**I want** a brief, unmistakable visual pulse on a folder or event when someone else with access changes it,
**so that** I notice changes made outside my own actions without having to open the audit log or get a separate notification for every edit.

---

## Behavior

- When a manager or staff member with granted access modifies a folder's contents (adds, moves, edits, or removes an event; renames the folder; changes its lock/visibility state), the affected folder row — and, if applicable, the specific event chip on the calendar grid — briefly **pulses/flashes**: a short scale/glow animation (roughly 400–600ms, 1–2 pulses), then returns to its normal resting state automatically. No manual dismissal needed.
- The pulse is purely presentational — it does not block interaction, does not require acknowledgment, and does not persist as a "changed" badge after the animation completes. Users who want a persistent record look at the audit log (`USER-STORY-audit-log.md`).
- The pulse triggers only for changes made by someone **other than** the currently signed-in user viewing the calendar — a user's own edits do not pulse (they already know they made the change).
- Change detection rides on the existing real-time notification fan-out (MASTER_SPEC §5.1 SignalR backbone, §6.4 activity fan-out) — this story does not introduce a new transport, only a new client-side reaction to an existing "folder/event changed" event.
- If multiple changes land in quick succession (e.g., a manager bulk-edits several events), each affected row pulses independently rather than pulsing the whole sidebar once — so the user's attention is drawn to the specific thing that changed.

## Settings

A single row, likely co-located in the same "Folder Locking" / "Folder Colors" Settings cards (`USER-STORY-folder-lock-settings.md`, `USER-STORY-folder-color-coding.md`) since it's another "how do I want to be told about external changes" control:

| Row | Control |
|---|---|
| Pulse on edit | Switch (default: on) |

---

## Acceptance criteria

- [ ] A manager/staff edit to a folder's contents pulses the affected folder row within the sidebar.
- [ ] A manager/staff edit to a specific event also pulses that event's chip on the calendar grid, if currently visible.
- [ ] The pulse animation completes and resets automatically without user interaction; no lingering "changed" state remains after it finishes.
- [ ] Edits made by the signed-in user themselves never trigger a pulse.
- [ ] Turning "Pulse on edit" off in Settings suppresses the animation entirely; the underlying change (and its audit-log entry) still occurs normally.
- [ ] Concurrent changes to multiple folders/events each pulse independently.

## Out of scope
- Sound effects or OS-level notifications for edits — this story is the in-app visual indicator only; audible/push alerts are covered by the broader notification system (MASTER_SPEC §5.1) and custom per-event alert packs (MASTER_SPEC §5.6), not redefined here.
- A persistent "unseen changes" badge/counter — the pulse is momentary by design; if a persistent indicator is wanted later, that's a new story, not an extension of this one.
