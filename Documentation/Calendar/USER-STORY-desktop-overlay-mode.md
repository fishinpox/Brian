# User Story — Desktop Overlay Mode

**Feature name:** Desktop Overlay Mode
**Status:** Draft — **flagged for architect review**, native-platform dependent
**Related:** [FrontEndLayout.md](FrontEndLayout.md) (the window this mode applies to)

> **Dependency flag:** transparency, click-through, always-on-background positioning, and global keybinds are native OS window behaviors. They cannot be implemented in a plain browser tab — they require the desktop wrapper (Electron on Mac / Tauri on Windows per `design-system.md`) or, for deeper OS integration, the Windows Agent (MASTER_SPEC §5.6, C#/.NET). This story specifies the desired behavior; the architect should confirm which layer (Electron/Tauri window APIs vs. Windows Agent) implements each part before build.

---

## Story

**As a** creator who wants their calendar visible at a glance without a dedicated window stealing focus,
**I want** the Calendar Window Overlay to sit on my desktop like a background layer — sized to my screen, click-through when I'm not interacting with it, and toggleable between passive and active states via keybind,
**so that** my schedule is always in view without getting in the way of whatever else I'm doing.

---

## Behavior

### Display modes
1. **Background (passive):** the overlay is sized to the user's full desktop resolution and rendered behind normal windows (or as a desktop-layer widget, akin to the Rainmeter-style widget layer referenced in MASTER_SPEC §5.6). Click-through is enabled — clicks pass to whatever is beneath the overlay.
2. **Foreground (active):** the overlay becomes a normal, focusable, click-through-disabled window — the user can interact with it (drag events, open Settings, switch tabs) exactly as described in `FrontEndLayout.md`.
3. **Solid:** same as foreground, but fully opaque (no desktop/wallpaper bleed-through) — for users who want it to behave like a conventional app window.
4. **Passive transparent scale:** an adjustable opacity level for the background mode — a slider/setting controlling how visible the overlay is when passive, from nearly invisible to a soft watermark-level presence.

### Toggling
- A **global keybind** (user-configurable) switches between background/passive and foreground/active modes without requiring the user to first bring another window into focus — this must work even when the overlay is currently click-through and not focused.
- Clicking into the overlay's content area while in background mode (if click-through is temporarily suspended via keybind) promotes it to foreground mode.

### Settings
| Row | Control |
|---|---|
| Overlay mode | Segmented control: Background / Foreground / Solid |
| Passive transparency | Slider (only relevant in Background mode) |
| Toggle keybind | Key-capture field to set/change the global shortcut |

---

## Acceptance criteria

- [ ] In Background mode, the overlay is sized to the desktop, renders behind normal application windows, and passes clicks through to whatever is underneath.
- [ ] In Foreground mode, the overlay behaves like a normal window: focusable, receives clicks, supports drag-and-drop and all `FrontEndLayout.md` interactions.
- [ ] Solid mode removes transparency entirely while keeping Foreground's interactivity.
- [ ] The passive transparency slider visibly changes the overlay's opacity in Background mode in real time.
- [ ] The configured global keybind toggles between modes regardless of which application currently has focus.
- [ ] Mode and transparency settings persist across app restarts.

## Out of scope
- macOS/mobile equivalents — per MASTER_SPEC §8.2, macOS customization ceiling is far lower (~25%, no System Integrity Protection bypass); this story is scoped to the Windows desktop experience. A reduced-scope macOS story is a future task if pursued.
- Exact implementation approach (native window flags vs. Windows Agent-level compositing) — left to the architect per the dependency flag above.
