# Front-End Layout — Calendar Window Overlay

**Status:** Draft spec (rewritten from original scratch notes, reconciled against the 2026-08-20 wireframe)
**Scope:** Layout and structure only. Behavior lives in the linked story files — this document does not redefine it.
**Related:**
- [USER-STORY-nested-folders-calendar.md](USER-STORY-nested-folders-calendar.md) — folder visibility/lock hierarchy, calendar grid, drag-and-drop
- [USER-STORY-desktop-overlay-mode.md](USER-STORY-desktop-overlay-mode.md) — the window's transparency/click-through/foreground behavior
- [USER-STORY-folder-lock-settings.md](USER-STORY-folder-lock-settings.md) — Settings-panel lock controls
- [USER-STORY-folder-color-coding.md](USER-STORY-folder-color-coding.md) — auto/time-sensitive folder coloring
- [USER-STORY-agenda-window.md](USER-STORY-agenda-window.md) — the Agenda Window panel
- [USER-STORY-event-edit-box.md](USER-STORY-event-edit-box.md) — reminders/flags shown in the right panel
- [USER-STORY-calendar-display-settings.md](USER-STORY-calendar-display-settings.md) — the mini-calendar/day-ticker widget
- Existing prototype: [.superdesign/design_iterations/mission_control_v2.html](../.superdesign/design_iterations/mission_control_v2.html)

---

## Goal

A single overlay window that lives on the user's desktop and gives at-a-glance access to their calendar, messages, and live streams without needing a browser tab. It opens on the **Calendar** view by default. The window shell (transparency, click-through, positioning) is specified separately in `USER-STORY-desktop-overlay-mode.md`; this document specifies what's *inside* the window.

---

## Window shell — view switcher

A top-level tab bar swaps the **entire window's content** between three views, reusing the exact pattern already built in `mission_control_v2.html` (`.tabs` / `#tab-cal`, `#tab-msg`, `#tab-holo` / `.view.active` on `#view-calendar`, `#view-messages`, `#view-holo`):

| Tab | Icon | Badge |
|---|---|---|
| **CALENDAR** | ▦ | — (default/active tab) |
| **MESSAGES** | ✉ | unread-count badge |
| **STREAMS** | ▶ | live-count badge (pulses while any followed stream is live) |

Only the **Calendar** tab is specified below. Messages and Streams reuse the existing prototype's layout and are not respec'd in this document.

---

## Calendar tab layout

Three-column shell: **Sidebar** (left, fixed width) · **Calendar grid** (center, flexible) · **Status panels** (right, fixed width) — with a persistent status bar pinned along the bottom of the right column.

### Left panel — Sidebar (Urgent, Folders, Alerts)

The sidebar is **three distinct stacked elements**, not one undifferentiated list:

1. **URGENT panel** (top, fixed — not part of the folder tree) — a dedicated window for immediate, time-critical matters that need the user's attention right now (e.g., an active chat awaiting a fast reply). Always visible; not collapsible. Each row shows a name and, optionally, a time-remaining badge (see below).

2. **Nested Folder tree** (middle, scrollable) — the general-purpose `FolderTree` component specified in `USER-STORY-nested-folders-calendar.md`: master folders → subfolders → items, with a visibility checkbox, a color-tinted icon, an expand/collapse chevron, and a **lock icon aligned to the right edge of the row** per master folder (`USER-STORY-folder-lock-settings.md`) — the same tree serves both chat-channel organization and calendar-item organization. Structure only (folder names, subfolders, and items are entirely user-defined — the labels below are placeholders, not fixed content):

   ```
   Master Folder
     Subfolder
       Item
       Item
     Subfolder
       Item
   ```

3. **ALERTS panel** (bottom, fixed — a separate window from the folder tree) — a customizable notification tray surfacing *dynamic* events pushed in from elsewhere in the system, not user-organized content: a streamer about to go live, a manager flagging a problem, a new friend/collab request, and similar. Which alert types show and how they're ordered is user-customizable (Settings details are a follow-up pass); the data source is the existing notification fan-out backbone (MASTER_SPEC §5.1/§6.4). Placeholder example of the row shape:

   ```
   Friend Requests [5]
   Collab Requests
     Request name [3w]
   ```

   > Note: "streamer about to go live" also drives the right panel's Agenda Window (below). These likely share one underlying data source surfaced in two places (a passive tray here vs. an active countdown widget there) — reconcile if they turn out to be redundant once both are built.

**Time-remaining badges** (`[1m]`, `[45m]`, `[3w]`, etc.) show time-until/time-since on rows in the URGENT panel and the ALERTS panel. Visibility and format of these badges is a **Settings toggle** — show, hide, or (later) customize — not hardcoded on.

### Center panel — Month progress + calendar grid

- **Month progress bar** — a header strip above the grid summarizing this month's task/event completion (e.g., "18 / 30 done"). Spans the center column's full width.
- **Calendar grid** ("Calendar 30 day") — the month grid, folder-colored event chips, overflow days, and drag-and-drop rules are the same mechanics specified in `USER-STORY-nested-folders-calendar.md` §Layout (right panel) and §Behaviors 3–4. Not redefined here.

### Right panel — four stacked status panels

Top to bottom:

1. **Progress of Today's Tasks** — compact stat panel: today's completion count/percentage. Static, no toggle state.
2. **Today's Tasks** — default state is a simple list. Has a toggle (referred to in the wireframe as "Toggle 2") that expands it into a day-strip agenda widget: previous/next-day navigation arrows, a 3-day date strip with the active day highlighted, a time-sorted list of the day's items (each row: time, title, a type tag), a "more" link for overflow, and a last-updated timestamp footer. List content is entirely the user's own scheduled items — there is no placeholder or seed content in the real product.
3. **Agenda Window** — a distinct panel (not a state of panel 2). Dynamic, timer/filter-driven list surfacing near-term events with live countdowns (e.g., "streamer goes live in 45 minutes"). Full behavior in `USER-STORY-agenda-window.md`.
4. **Team Users Online / Major Events Scheduled** — one panel, multiple toggle states (wireframe's "Toggle 3"):
   - **Default state:** team-presence list — who's online now.
   - **Toggled state:** a mini month-calendar (day-ticker style, current day highlighted) stacked above a flagged/sortable reminders list ("Arrange by: Flag / Due Date / Today"), grouped under headers like "Today." Mini-calendar spec in `USER-STORY-calendar-display-settings.md`; reminder/flag data model in `USER-STORY-event-edit-box.md`.

### Bottom status bar

Persistent strip pinned under the right column (visible regardless of which right-panel toggle states are active): user avatar + display name, a mic mute toggle, a headset/deafen toggle, and a **gear icon**. The gear icon is the entry point into **Settings**, where the lock toggle (`USER-STORY-folder-lock-settings.md`), color customization (`USER-STORY-folder-color-coding.md`), audit-log history size (`USER-STORY-audit-log.md`), time-badge visibility, and account/security controls (`USER-STORY-account-security-settings.md`) all live.

---

## Acceptance criteria

- [ ] The Calendar/Messages/Streams tab bar swaps the entire window's content; the previously active view's state is preserved when switching back.
- [ ] Calendar tab renders all three columns plus the bottom status bar simultaneously; no column is scrollable in a way that hides another column.
- [ ] Left sidebar renders as three distinct elements — URGENT panel, Nested Folder tree, ALERTS panel — not one merged list; folder rows carry visibility checkboxes, colored icons, expand/collapse, and a right-aligned lock icon, including newly created folders.
- [ ] Time-remaining badges in the URGENT and ALERTS panels respect the Settings show/hide toggle.
- [ ] Center panel renders the month progress bar above the calendar grid; grid behavior matches `USER-STORY-nested-folders-calendar.md`.
- [ ] All four right-column panels render in the specified order; panels 2 and 4 each support their documented toggle states without affecting panels 1 or 3.
- [ ] Status bar remains visible and functional (mic/headset toggles, gear→Settings) regardless of which tab or toggle state is active.

## Out of scope for this document

- Transparency/click-through/keybind behavior of the window itself — see `USER-STORY-desktop-overlay-mode.md`.
- Messages and Streams tab content — reuse `mission_control_v2.html`, not respec'd here.
- Full behavioral rules for locking, coloring, the audit log, the agenda window, reminders, alert customization, and account security — each lives in its own story file linked above.
