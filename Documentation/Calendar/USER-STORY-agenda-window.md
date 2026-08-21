# User Story — Agenda Window

**Feature name:** Agenda Window
**Status:** Draft
**Related:** [FrontEndLayout.md](FrontEndLayout.md) (right-panel placement, panel 3 of 4), [USER-STORY-event-edit-box.md](USER-STORY-event-edit-box.md) (reminder data model it may draw from)

---

## Story

**As a** creator juggling my own schedule and the streamers/collaborators I follow,
**I want** a single dynamically-updating panel that surfaces what's coming up soon with live countdowns and lets me filter by type,
**so that** I don't miss something time-sensitive (a stream going live, a deadline, a meeting) buried in a static list.

---

## Layout

A dedicated panel in `FrontEndLayout.md`'s right column (panel 3 of 4, between "Today's Tasks" and "Team Users Online / Major Events Scheduled") — distinct from those panels, not a toggle state of either.

- A filter control (type/category — e.g., "My schedule," "Followed streamers," "Deadlines," "All") narrows the list.
- Each row: item name, source/type indicator, and a **live-updating countdown or elapsed-time string** (e.g., "goes live in 45 minutes," "started 12 minutes ago," "due in 2 hours").
- Rows re-sort automatically as time passes — soonest-first.

## Behavior

- Countdown strings update in real time (client-side ticking, not requiring a fresh server fetch every second) — the underlying due/start time is fetched normally, but the display recalculates the relative-time string on an interval (e.g., every 30–60 seconds is sufficient for a "45 minutes" granularity; no need for second-level precision).
- Items entering their "imminent" window (configurable, e.g., within 1 hour) get a visual emphasis (e.g., accent color/subtle pulse) distinct from the general list — but this is a separate mechanism from the live-edit pulse in `USER-STORY-live-edit-pulse.md`, which is about *manager edits*, not time-proximity.
- Data sources feeding this panel: the user's own calendar events, followed streamers' schedules (per MASTER_SPEC §5.2's streamer-following calendar), and reminders/countdowns from `USER-STORY-event-edit-box.md`. This story defines the aggregating display, not each individual data source.
- Dismissing/hiding a specific item from the Agenda Window (without deleting the underlying event) is a nice-to-have, not required for v1.

---

## Acceptance criteria

- [ ] The Agenda Window renders as its own panel in the right column, distinct from "Today's Tasks" and "Team Users Online."
- [ ] Countdown/elapsed-time strings update automatically without requiring a manual refresh or page reload.
- [ ] The list re-sorts as items move closer to or past their due/start time.
- [ ] The type/category filter narrows the visible list without affecting the underlying data.
- [ ] An item entering its "imminent" window is visually distinguishable from items further out.

## Out of scope
- Push notifications when something in the Agenda Window becomes imminent — that's the existing notification fan-out (MASTER_SPEC §5.1), not this panel's job; this panel is a passive, glanceable surface.
- Cross-referencing/deduplicating against the sidebar's ALERTS panel (`FrontEndLayout.md` flags this as a likely overlap to reconcile) — left for a follow-up pass once both are built.
