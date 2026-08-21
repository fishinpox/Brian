# User Story — Event Edit Box & Plugin Menu

**Feature name:** Event Edit Box
**Status:** Draft
**Related:** [FrontEndLayout.md](FrontEndLayout.md) (reminders/flags feed the right panel's Toggle-3 state), [USER-STORY-agenda-window.md](USER-STORY-agenda-window.md) (consumes reminder/countdown data), [MASTER_SPEC.md](MASTER_SPEC.md) §5.7 (Productivity Core — recurrence/tasks already covered there at the product level)

---

## Story

**As a** creator recording everything from trivial notes to hard deadlines,
**I want** one consistent edit surface for calendar days/events — quick memos, full notes, reminders, countdowns, completion, recurrence, and search — reachable from a small, consistent set of entry points,
**so that** I'm not hunting for different UI for closely related actions.

This story consolidates a batch of small, related features (originally scouted from a reference calendar app's feature list) into one cohesive spec, matched to functionality per MASTER_SPEC's rule to reference, not clone.

---

## Layout — two entry points

1. **Top-right plugin menu** (small button in the calendar view's corner): opens **Note** management (create/configure notes) and **Search** (schedule history). Distinct from the Calendar → Display menu in `USER-STORY-calendar-display-settings.md`.
2. **Event edit box** (opened by double-clicking a date, or opening an existing event): a compose surface with dedicated buttons for **Repeat**, **Reminder**, and **Countdown**, plus a **Finish/complete** control on existing entries.

---

## Behavior 1 — Calendar Memo

- Double-clicking any date opens a lightweight inline edit box for quick, concise entries — pressing Enter starts a new line for an additional entry on the same date, without leaving the box.
- Distinct from the full Note feature (Behavior 2): Memo is for short trivia tied to a specific date; Note is a richer, freestanding document.

## Behavior 2 — Note

- Accessible from the top-right plugin menu: create a new note or configure/edit an existing one.
- Notes are not necessarily tied to a single date the way Memos are — they're a general lightweight document feature attached to the calendar context.

## Behavior 3 — Reminder

- Added via a "Reminder" button inside the event edit box.
- Configurable: recurrence type, end date, number of reminders, and reminder time(s) — full personalization, not a single fixed alert.
- Reminder entries are the data source for the flagged/sortable list shown in `FrontEndLayout.md`'s right-panel Toggle-3 state ("Arrange by: Flag / Due Date / Today").

## Behavior 4 — Countdown

- Added via a "Countdown" button inside the event edit box.
- Five categories, each with distinct card styling: **Birthday, Holiday, Anniversary, Exam, Default**.
- Countdown cards can feed `USER-STORY-agenda-window.md`'s aggregated list as one of its data sources.

## Behavior 5 — Finish (mark complete)

- A one-click "complete" control on any task/event in its edit view, giving immediate visual confirmation (e.g., strikethrough/checkmark state) rather than requiring a full edit-and-save round trip.

## Behavior 6 — Repeat / recurrence

- A "Repeat" button in the event edit box configures recurrence type and end conditions.
- The previously-separate "Class Schedule" idea (a recurring weekly timetable) is **folded into this recurrence engine** as a preset/template rather than a distinct feature — a weekly class-style schedule is just a recurring event with a weekly pattern; no separate timetable UI is built.

## Behavior 7 — Search

- Accessible from the top-right plugin menu: a keyword search bar filtering the schedule/event history, results displayed below the search bar.
- Double-clicking a search result opens that event's edit box (Behavior links back to the base edit surface, not a separate viewer).

## Behavior 8 — Auto-defer

- Tasks left incomplete are automatically rolled forward to the next day.
- The auto-defer window looks back up to **3 days** for unfinished tasks and extends them forward; once a task has been auto-deferred for **7 consecutive days**, deferral stops (the task is no longer automatically pushed forward — it stays put until the user acts on it).
- User-toggleable: unchecking auto-defer for a given task stops it from being pushed forward again.

---

## Acceptance criteria

- [ ] Double-clicking a date opens the Memo edit box; pressing Enter adds a new line/entry without closing the box.
- [ ] The top-right plugin menu provides access to Note management and Search, separate from Calendar → Display settings.
- [ ] Reminder configuration supports recurrence type, end date, reminder count, and reminder time(s).
- [ ] Countdown supports at least the five specified categories with visually distinct cards.
- [ ] Marking a task Finished updates its visual state immediately without a full page/view reload.
- [ ] Repeat/recurrence configuration covers weekly-pattern use cases well enough that no separate "Class Schedule" feature is needed.
- [ ] Search filters the schedule by keyword and results open directly into the edit box on double-click.
- [ ] An incomplete task auto-defers up to 7 consecutive days, then stops auto-deferring; the user can opt a task out of auto-defer entirely.

## Out of scope
- Rich text/attachments in Notes — start with plain text, revisit if requested.
- Cross-device conflict resolution for simultaneous edits to the same Memo/Note — assume last-write-wins for v1, consistent with how MASTER_SPEC's sync backbone generally behaves unless a specific conflict policy is requested.
