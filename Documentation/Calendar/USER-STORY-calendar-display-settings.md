# User Story — Calendar Display Settings

**Feature name:** Calendar Display Settings
**Status:** Draft
**Related:** [USER-STORY-nested-folders-calendar.md](USER-STORY-nested-folders-calendar.md) (the grid this modifies), [FrontEndLayout.md](FrontEndLayout.md) (mini-calendar used in the right panel's Toggle-3 state)

---

## Story

**As a** creator whose week doesn't respect month boundaries and who doesn't work every day,
**I want** to see a continuous run of days across month boundaries instead of being boxed into strict calendar months, and to hide/blackout the days and hours I'm not working,
**so that** my calendar reflects how I actually plan (in weeks, around real availability) rather than the calendar grid's arbitrary boundaries.

---

## Behavior 1 — Floating months (cross-month display)

- A **Calendar → Display** settings menu (opened via a small arrow/menu control in the calendar view's top-right corner, matching the described reference pattern) offers a toggle between:
  - **Full-month display** (default) — the existing strict month grid from `USER-STORY-nested-folders-calendar.md`, with overflow days from adjacent months shown faded at the grid's edges.
  - **Cross-month / floating display** — the grid scrolls continuously by week rather than resetting at month boundaries, so the user can see the previous or next week(s) regardless of where the current month starts or ends, without navigating away from the current view.
- Switching modes does not change any event data — it's purely a display/navigation mode. Drag-and-drop, folder coloring, and lock rules from the base calendar story apply identically in both modes.

## Behavior 2 — Hide Rest Days / blackout dates & hours

- A setting to hide designated **rest days** from the grid entirely, so the view shows only working days — for a user who wants to focus on workdays without rest-day clutter.
- A related, separate setting for **blackout dates/hours** — e.g., recurring sleep hours or specific date ranges — blocked out visually on the grid (and, where relevant to time-based views like the day-strip agenda in `FrontEndLayout.md`'s right panel, excluded from time-slot displays).
- Rest-day and blackout configuration lives in the same Calendar → Display settings menu as floating months, since the user's own reference material groups both under that menu.

## Behavior 3 — Mini month-calendar / day-ticker widget

- A compact calendar widget (month grid at reduced scale, current day highlighted, prev/next month arrows) used inside `FrontEndLayout.md`'s right-panel Toggle-3 state ("Team Users Online / Major Events Scheduled").
- Selecting a day in the mini-calendar filters the reminders list stacked beneath it (data model in `USER-STORY-event-edit-box.md`) to that day, without navigating the main center-panel calendar grid away from its current view — the mini-calendar is a secondary, independent navigation surface.

---

## Acceptance criteria

- [ ] Calendar → Display settings menu is reachable from a small arrow/menu control in the calendar view's top-right corner.
- [ ] Toggling cross-month display shows days from adjacent weeks/months in a continuous scroll without a hard month-boundary reset; toggling back to full-month display restores the standard grid from `USER-STORY-nested-folders-calendar.md`.
- [ ] Hiding rest days removes them from the grid entirely (not just greys them out) when enabled.
- [ ] Blackout dates/hours are visually distinct from normal available time and excluded from relevant time-slot pickers.
- [ ] The mini-calendar widget in the right panel's Toggle-3 state highlights the current day and supports month navigation independently of the main grid.
- [ ] Selecting a day in the mini-calendar filters the adjacent reminders list to that day.

## Out of scope
- Defining which days count as "rest days" by default (e.g., auto-detecting weekends vs. fully manual) — left as a build-time decision, likely manual selection per day-of-week as a starting point.
- Recurring exception handling (e.g., "rest day except every third Saturday") — basic on/off per day-of-week and simple date-range blackout only, for v1.
