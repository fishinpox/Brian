# User Story — Folder Lock Settings & Manager Override

**Feature name:** Folder Lock Settings
**Status:** Draft
**Related:** [USER-STORY-nested-folders-calendar.md](USER-STORY-nested-folders-calendar.md) (base lock hierarchy), [USER-STORY-audit-log.md](USER-STORY-audit-log.md) (shares a Settings card), [FrontEndLayout.md](FrontEndLayout.md) (gear icon entry point)

---

## Story

**As a** creator who occasionally drags things by accident,
**I want** the Master Lock / per-folder lock behavior from the nested-folder calendar to be configurable from Settings — not just toggled inline — and I want approved Managers to be able to edit a locked event from outside the app without unlocking it for me,
**so that** I don't have to choose between "always locked, annoying" and "always unlocked, risky," and my managers aren't blocked by a safety feature that exists for my benefit, not theirs.

---

## Layout

### Per-folder lock icon placement
On every master folder row in the sidebar (`USER-STORY-nested-folders-calendar.md`'s folder tree), the lock icon sits **flush right on the row**, after the folder name — same row, opposite side from the visibility checkbox. This placement applies to every master folder, including ones created after this feature ships (no separate "add lock" step — new folders get the control automatically).

### Settings — "Folder Locking" card
A new card in the Settings panel (opened via the gear icon in `FrontEndLayout.md`'s status bar), styled as a section header + row-card list (icon, label, subtitle, trailing control per row — same structural pattern as a typical grouped settings screen: a header row, then a bordered card containing stacked rows separated by thin dividers, each with an icon, a label/subtitle pair, and a trailing switch/stepper):

| Row | Control |
|---|---|
| Lock Folders | Switch — global default for whether newly created folders start locked or unlocked |
| Manager Override | Switch — whether approved Managers can edit locked events from outside the app (see Behavior 2) |
| History size | Stepper, 50–500 (shared with `USER-STORY-audit-log.md` — same card, since both are "who can touch my calendar and how much do I remember about it") |

---

## Behavior 1 — Settings-backed lock defaults

- The inline Master Lock/per-folder lock checkboxes (`USER-STORY-nested-folders-calendar.md` Behavior 2) remain the moment-to-moment controls a user actually clicks.
- The Settings "Lock Folders" switch sets the **default lock state for newly created folders** — it does not retroactively change existing folders' lock state.
- Turning "Lock Folders" off in Settings does not force-unlock anything already locked; it only changes the default going forward.

## Behavior 2 — Manager override

- A Manager with granted permissions on a talent's calendar (per MASTER_SPEC §4's manager↔creator relationship) can edit an event even while that event's folder is locked, **without unlocking it** — the lock only blocks the calendar owner's own accidental drag-and-drop inside the app; it is not a permission boundary against an authorized Manager.
- This override is gated by the "Manager Override" Settings switch (default: on, since the manager relationship already implies granted permissions per MASTER_SPEC §4). Turning it off blocks even authorized Managers from bypassing a lock — an explicit escape hatch for a creator who wants a genuinely hard lock.
- Manager edits made this way still go through the normal permission/audit path — they show up in the audit log (`USER-STORY-audit-log.md`) and trigger the live-edit pulse (`USER-STORY-live-edit-pulse.md`) exactly like any other manager-originated change.
- Locked-event drag protection inside the calendar app itself is unaffected — the override only applies to edits made through the Manager's own tooling/permissions, not to dragging inside the owner's calendar view.

---

## Acceptance criteria

- [ ] Every master folder row (existing and newly created) shows a lock icon flush right, independent of the visibility checkbox on the left.
- [ ] Settings → Folder Locking shows Lock Folders, Manager Override, and History size in one card.
- [ ] Toggling "Lock Folders" in Settings changes only the default for folders created after the change; it does not alter existing folders.
- [ ] With Manager Override on, an authorized Manager can edit a locked event via their own tooling without the calendar owner unlocking it first; the edit appears in the audit log.
- [ ] With Manager Override off, the same edit attempt is rejected even for an authorized Manager.
- [ ] The lock icon's visual states (open/green vs. closed/red, per `USER-STORY-nested-folders-calendar.md`) are unchanged by this feature — this story only adds the Settings layer and the manager exception.

## Out of scope
- The visual/interaction design of the Settings panel itself beyond this one card (full Settings IA is a separate pass).
- Granting/revoking manager permissions (already covered by MASTER_SPEC §4's role model) — this story only consumes that permission, it doesn't define how it's granted.
