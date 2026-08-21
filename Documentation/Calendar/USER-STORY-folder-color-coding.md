# User Story — Folder Color Coding & Time-Sensitivity Recoloring

**Feature name:** Folder Color Coding
**Status:** Draft
**Related:** [USER-STORY-nested-folders-calendar.md](USER-STORY-nested-folders-calendar.md) (color-as-source-of-truth for folder membership), [USER-STORY-folder-lock-settings.md](USER-STORY-folder-lock-settings.md) (shares the Settings surface)

---

## Story

**As a** creator with a growing number of folders,
**I want** new folders to get a sensible color automatically, events to recolor instantly when they move folders, and folder/event color to optionally shift as a deadline gets closer,
**so that** I don't have to manually pick a color for every folder I create, and a glance at color alone tells me both "which folder" and "how urgent."

---

## Behavior 1 — Auto color assignment

- When a new master folder is created, it's automatically assigned the next unused color from a fixed palette (extending the existing 3-folder palette in `USER-STORY-nested-folders-calendar.md` — blue/purple/green — with additional swatches as folder count grows).
- If all palette colors are in use, colors are reused in creation order (oldest folder's color repeats) rather than generating arbitrary new colors, so the palette stays visually consistent.
- Auto-assigned color follows the same background/text/border triplet structure as the existing palette (e.g., `#E3F2FD` / `#1976D2` / `#2196F3`).

## Behavior 2 — Recolor on drop

- When an event/item is dragged into a different master folder (per `USER-STORY-nested-folders-calendar.md` Behavior 3c), its chip's background/border/text recolor **immediately** to the destination folder's palette — this is already specified behavior in the base story; this story adds that the same recolor-on-drop rule applies uniformly regardless of *why* the item entered the folder (manual drag, a new event created directly inside the folder, or a collab-hub event landing there via `USER-STORY-collab-event-hub.md`).

## Behavior 3 — Time-sensitivity recoloring (opt-in)

- A Settings toggle ("Auto-recolor by time sensitivity") lets folder color escalate as an event's due date approaches, independent of its folder-membership color:
  - **Normal:** folder-membership color (default, unchanged from base story).
  - **Due soon** (within 24h of the event's time, configurable): color shifts toward an amber/orange accent, layered as a border/glow rather than replacing the folder's base color outright — so folder identity stays legible.
  - **Overdue:** shifts to a red accent.
- This is per-event, not per-folder: a folder's events can be in different escalation tiers simultaneously. The folder icon/row itself reflects the most urgent tier among its visible events (e.g., a folder with one overdue event shows a small red indicator even if its base color is blue).
- Toggle default: **off** — this is a deliberate opt-in, since some users will find the base folder-color-only scheme sufficient (folder color is the primary "source of truth" per `USER-STORY-nested-folders-calendar.md`, and this feature layers urgency on top rather than replacing it).

## Behavior 4 — Settings

New rows in the Settings → "Folder Colors" card:

| Row | Control |
|---|---|
| Folder color | Per-folder swatch picker — manual override of the auto-assigned color |
| Auto-recolor by time sensitivity | Switch (default off) |
| Escalation thresholds | Two time inputs — "due soon" window and (implicitly) overdue = past due time |

---

## Acceptance criteria

- [ ] Creating a new master folder assigns it the next unused palette color with no user action required.
- [ ] Dragging an event into a different folder recolors its chip to the destination folder's palette instantly, matching `USER-STORY-nested-folders-calendar.md`'s existing recolor-on-drop rule.
- [ ] With time-sensitivity recoloring off (default), no event's color changes based on time — only folder membership determines color.
- [ ] With time-sensitivity recoloring on, an event crossing into its "due soon" window visually escalates without losing its folder-color identity; crossing overdue escalates further.
- [ ] A folder's row/icon reflects its most urgent contained event's tier when time-sensitivity recoloring is on.
- [ ] Manually overriding a folder's color via Settings persists and is not overwritten by future auto-assignment.

## Out of scope
- Configurable custom palettes beyond the fixed swatch set (user-defined arbitrary hex colors) — future enhancement.
- Colorblind-safe palette variants — flag for the accessibility pass referenced in `design-system.md` §Accessibility, not solved here.
