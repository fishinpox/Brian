# User Story — Nested Folders with Calendar Integration, Locks & Month Navigation

**Feature name:** Nested Folder Calendar System
**Status:** Prototyped and approved (interactive mockup iterated 2026-07-15)
**Related stories:** Story 1 (unified calendar), Story 2 (kanban tasks) in USER-STORIES.md

---

## Story

**As a** VTuber managing collabs, sponsor work, and corporate projects,
**I want** my calendar events organized into renameable master folders with nested subfolders — each level having its own visibility toggle and drag-lock — and the ability to drag events between folders, between dates, and between months,
**so that** I can visually isolate any slice of my schedule, reorganize events safely without accidental moves, and manage events across month boundaries without leaving the calendar view.

---

## Layout (two-panel)

### Left panel — "Calendar Folders" sidebar
Fixed-width (~300px) scrollable panel containing, top to bottom:

1. **Panel header:** "Calendar Folders"
2. **Master Lock control** — a single row with a lock icon + label + checkbox. Sits above all folders.
3. **Master folders** (e.g., Master Folder A / B / C), each rendered as a card row containing:
   - Visibility checkbox (checked = visible)
   - Folder icon tinted in the folder's theme color
   - Folder name (renameable at any time — rename must propagate everywhere the name displays)
   - Expand/collapse chevron (▼ expanded / ▶ collapsed); clicking the row toggles expansion
4. **Subfolders** nested inside each master folder (e.g., Collabs under A, Sponsors under B, Projects under C), each with:
   - Visibility checkbox
   - Folder icon in the parent's theme color
   - Subfolder name
   - **Lock icon + lock checkbox** (per-subfolder drag lock)
5. **Individual events** listed under their subfolder, each with:
   - Its own visibility checkbox
   - Event name label
6. **Drop zone per master folder:** the area containing a master folder's subfolder(s) and events is a dashed-border drop target tinted with the folder's color at low opacity. Dragging an event from anywhere (calendar or another folder) onto this zone re-assigns the event to that master folder.

### Right panel — Calendar view
1. **Month navigation header** — three elements in a row:
   - **Previous-month drop zone** (left): dashed orange border, left-arrow icon, labeled with the actual previous month name (e.g., "June 2026")
   - Current month title, centered (e.g., "July 2026")
   - **Next-month drop zone** (right): dashed green border, right-arrow icon, labeled with the next month name (e.g., "August 2026")
2. **Status message bar** — hidden by default; appears below the header on every successful drop, then auto-dismisses (~2.5s)
3. **Month grid** — standard Sun→Sat table. Each day cell shows the day number and holds event chips. The grid includes **overflow days** from adjacent months in their true positions (e.g., July 2026 starts Wednesday, so June 28–30 fill the first Sun–Tue; August 1 lands on the final Saturday)
4. **Legend/help card** below the grid summarizing the drop-zone rules

---

## Event chips

- Small rounded chips inside day cells: folder-tinted background, matching border and text color, folder color = source of truth for which master folder the event belongs to
- Grab cursor when draggable; `not-allowed` cursor + reduced opacity (~0.6) when locked
- While being dragged, the chip's opacity drops to 0.5 and restores on drag end
- When an event is re-assigned to a different master folder, its background/border/text colors immediately update to the new folder's palette

### Color scheme used in the prototype
| Folder | Background | Text | Border |
|---|---|---|---|
| Master Folder A (Collabs) | #E3F2FD | #1976D2 | #2196F3 (blue) |
| Master Folder B (Sponsors) | #F3E5F5 | #7B1FA2 | #9C27B0 (purple) |
| Master Folder C (Projects) | #E8F5E9 | #388E3C | #4CAF50 (green) |
| Previous-month accents | — | — | #FF9800 (orange) |
| Next-month accents | — | — | #4CAF50 (green) |

---

## Behavior 1 — Visibility toggles (three levels)

Visibility is independent of locking. Three toggle levels, each a checkbox:

1. **Master folder toggle:** unchecking hides ALL events belonging to that master folder from the calendar (regardless of subfolder). Rechecking restores them.
2. **Subfolder toggle:** unchecking hides all events belonging to that subfolder only. Other subfolders in the same master folder are unaffected.
3. **Individual event toggle:** unchecking hides only that one event from the calendar.

**Critical implementation notes (bugs hit during prototyping — do not repeat):**
- Toggling an event's checkbox must hide the **calendar chip only**. The sidebar row (checkbox + label) must remain visible so the user can re-check it. The bug: the toggle hid its own sidebar row, making the state unrecoverable.
- Visibility state must be **explicitly driven by the checkbox's checked state** (checked ⇒ show, unchecked ⇒ hide), not by blindly flipping the current display value. Blind-flip logic desynchronizes when multiple toggle levels affect the same event.
- Checkbox clicks inside clickable folder rows need `stopPropagation` so they don't also trigger row expand/collapse.

## Behavior 2 — Lock hierarchy (drag protection)

Locks prevent **dragging only**. They never affect visibility toggles.

**Hierarchy and permission rules (in priority order):**
1. **Master Lock (top of sidebar):** when checked, ALL events everywhere are undraggable, and every subfolder lock control becomes disabled/greyed (their checkboxes cannot be changed while master is locked).
2. **Subfolder locks:** usable only while the Master Lock is unchecked. Locking a subfolder makes only that subfolder's events undraggable; other subfolders stay draggable.
3. An event is draggable **if and only if** the Master Lock is off AND its subfolder's lock is off.

**Lock state visuals:**
- Unlocked: open-padlock icon, green (#4CAF50)
- Locked: closed-padlock icon, red (#E53935)
- Locked events: `not-allowed` cursor, 0.6 opacity, `draggable=false`, and drag-start is also prevented in code (belt and suspenders — don't rely on the attribute alone)

**Lock is keyed to the event's subfolder membership** (a data attribute on the chip), so an event dropped into a locked context immediately obeys the new context's lock state. Re-evaluate all chips' drag state after every drop and after every lock change.

## Behavior 3 — Drag & drop (three target types)

An event chip can be dropped on:

### a) Any day cell in the calendar grid
- Chip moves into the cell (appended after existing chips)
- The message shown depends on the **day the cell represents — never on which weekday column it is in**:
  - **Current-month day** (including Saturdays and Sundays): "Moved to July 8", "Moved to July 5", etc.
  - **Overflow day representing the previous month** (tinted orange, e.g., June 28–30 at the start of the grid): "✓ Moved to June 30 (previous month)"
  - **Overflow day representing the next month** (tinted green, e.g., August 1 on the final Saturday): "✓ Moved to August 1 (next month)"
- **Bug hit during prototyping — do not repeat:** month-move behavior was originally keyed to the Sunday/Saturday **columns**, which wrongly reported month changes for current-month weekend dates. Weekend columns are ordinary drop targets; only a cell's month membership decides the message.

### b) Header month zones (the ONLY month-only drop targets)
- Dropping on the orange previous-month zone: "✓ Moved to June 2026"; on the green next-month zone: "✓ Moved to August 2026"
- The event is removed from the current month's grid on drop (it now lives in a month not being displayed). In the real product this means: update the event's date by ∓1 month (same day-of-month, clamped to that month's length), persist, and re-render.
- Hover feedback: zone background intensifies and zone scales up slightly (~1.05); resets on leave/drop.

### c) Master folder drop zones in the sidebar
- Dropping re-assigns the event's master folder: its `data-folder` updates and its chip recolors to the target folder's palette instantly
- The chip appears inside the sidebar drop zone in the prototype; in the real product the event keeps its date and only changes folder membership (recolor on the calendar in place)

### General drag rules
- Locked events cannot start a drag at all
- Every valid drop target highlights on `dragover` (cells: surface tint + 1px ring; zones: intensified tint) and resets on `dragleave`/`drop` — overflow cells must reset to their orange/green base tint, not to transparent
- After any drop, re-run the lock-state evaluation across all chips

## Behavior 4 — Month grid with overflow days

- The grid always renders full weeks; leading/trailing cells belong to the adjacent months
- Overflow cells are visually distinct: previous-month days get a faint orange background with orange day numbers; next-month days get faint green with green day numbers
- Overflow cells are live drop targets that perform a month move (see 3a)
- Day-of-week columns carry no special drop semantics of their own

---

## Acceptance criteria

**Visibility**
- [ ] Unchecking a master folder hides all its events on the calendar; rechecking restores exactly those events
- [ ] Unchecking a subfolder hides only that subfolder's events
- [ ] Unchecking an individual event hides only that chip; its sidebar row (checkbox + name) stays visible and re-checkable
- [ ] Any combination of the three levels resolves without desync (event visible ⇔ its own toggle AND its subfolder toggle AND its master toggle are all checked)

**Locks**
- [ ] With Master Lock on: no event can be dragged; all subfolder lock controls are disabled/greyed
- [ ] With Master Lock off: each subfolder lock independently freezes only its own events
- [ ] Locked chips show not-allowed cursor + dimmed style; drag-start is programmatically blocked
- [ ] Turning Master Lock off restores each subfolder's own prior lock state (subfolder locks are not cleared by the master)

**Drag & drop**
- [ ] Dragging between two current-month dates — including Sat/Sun — shows "Moved to [Month] [day]" with no month-change wording
- [ ] Dropping on an overflow day moves the event to that specific adjacent-month date and says so
- [ ] Dropping on a header month zone moves the event to that month and removes it from the current view
- [ ] Dropping on a sidebar master-folder zone re-assigns the folder and recolors the chip instantly
- [ ] Every drop shows a status message that auto-dismisses; every hover target highlights and cleanly resets

**Folders**
- [ ] Master folders can be renamed at any time; the new name propagates to all displays
- [ ] Folder tree supports expand/collapse per master folder without affecting visibility state

---

## Out of scope for this story (future)
- Persistence/backend (prototype is in-memory; production needs folder + event + lock + visibility state saved per persona)
- Arbitrary-depth nesting (prototype covers master → subfolder → events; deeper nesting TBD)
- Multi-select drag, keyboard shortcuts, right-click context menus (rename/delete/move), undo
- Recurrence interaction with folder moves
