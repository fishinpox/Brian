# Calendar Front-End Technology Choices

**Purpose:** Record the technology decisions for the Calendar UI front end — replacing the current static `wwwroot` pages with a proper animated, user-themeable client.

**Last Updated:** July 2026 | **Version:** 1.0

**Status:** Decision made. Not yet scaffolded/implemented.

---

## Goal

Build a calendar front end that:

- Displays events pulled from `Calendar.API`.
- Supports rich UI animation (event transitions, view changes, background-image crossfade) driven natively by state, not bolted on after the fact.
- Lets users pick their own font.
- Lets users upload a custom background image (backend already exists — see [BackgroundImage.md](BackgroundImage.md)).

The project is starting from scratch on the front end, so there was no constraint to keep the existing static-HTML approach — the full stack was open for reconsideration.

## Decision

| Concern | Choice |
|---|---|
| Framework | **React + TypeScript + Vite** |
| Calendar grid/scheduling engine | **FullCalendar**, via its official `@fullcalendar/react` wrapper |
| Animation | **Motion** (motion.dev, formerly Framer Motion) — React binding, Motion+ premium tier under consideration |
| Styling / user font selection | **Tailwind CSS** + a CSS custom property (`--font-family`) swapped by a user setting |
| Data fetching/caching | **TanStack Query** against `Calendar.API` |
| Real-time updates | `@microsoft/signalr` against the existing Notifications hub (`/hubs/notifications`) |

## Why this combination

- **React over Svelte/Vue:** Svelte has the most "native" animation model (transitions are a language feature), and Vue's `<Transition>` is also framework-native, but React has the deepest library support for everything else in this stack (FullCalendar's React wrapper, Motion's most mature integration, largest hiring/ecosystem pool). Given the rest of the stack is C#/.NET, TypeScript was preferred for the type-safety parity.
- **FullCalendar instead of a hand-rolled grid:** recurrence rules, drag/drop, multi-view (month/week/day/list), and timezone handling are substantial, easy-to-get-wrong logic. FullCalendar solves this; a custom grid would only be justified if a specific animation (e.g., a morphing month→week transition) turns out to be impossible on top of it.
- **Motion for animation:** FullCalendar has no animation engine of its own — it renders plain DOM with lifecycle hooks (`eventDidMount`, `eventWillUnmount`, `datesSet`). Motion hooks into those (and into React state generally) via `AnimatePresence`/layout animations, giving state-driven transitions rather than CSS-class-toggle hacks.
- **Motion+ (paid tier) fits this stack specifically:** Motion officially supports React, Vue, and vanilla JS (not Svelte), and its tooling/examples are most mature for React — the framework already chosen here.

## Options considered and ruled out

- **Framer.com** — looked similar in name to Motion, but is a *different, unrelated product*: a no-code AI website builder for marketing sites/landing pages/CMS-driven blogs. It cannot connect to a custom JWT-authenticated backend or a SignalR hub, so it doesn't apply to building the Calendar app itself. (Framer, the company, is where Motion originated as "Framer Motion" before being spun out — that's the only real connection.)
- **C# Blazor Hybrid (WPF/MAUI + BlazorWebView)** — viable technically (BlazorWebView is Chromium/WebView2 under the hood, so CSS animation/fonts work fine), and would keep everything in C#. Not chosen for the *web* Calendar UI because it doesn't fit a browser-delivered app; it was evaluated in this conversation primarily in the context of a separate, unrelated tangent (a native Windows desktop-shell/ricing project, see below) rather than the Calendar front end.
- **Hand-rolled grid + CSS View Transitions API** — would give maximum animation control but requires rebuilding recurrence/drag/timezone logic FullCalendar already provides. Deferred unless a specific animation need can't be achieved on top of FullCalendar.

## Architecture implication

This replaces the current static-HTML-in-`wwwroot` pattern used by `calendar.html`, `holodex-follow.html`, and `youtube-follow.html` in `Calendar.API`. The new front end is a standalone Vite-built SPA (its own `package.json`, dev server, build output) that calls `Calendar.API`, `Identity.API` (JWT auth), and `Notifications.API` (SignalR) over HTTP — either routed through the Gateway (YARP) as a new route, or hosted separately. This is a structural change, not a drop-in library swap, and is a separate implementation task from this decision record.

## Unrelated tangent (not part of this decision)

Mid-conversation, two Windows desktop-customization projects were evaluated (`end4-pC` / Quickshell-Hyprland dotfiles, and WindHawk) against the idea of building a native Windows desktop shell/widget bar in C#/Blazor. This was **not** about the Calendar web UI — it's a separate, unrelated idea and is noted here only so it isn't confused with the Calendar front-end decision above. No decision or commitment was made on that topic.

## Next Steps

- Scaffold the Vite/React/TypeScript project.
- Wire up `@fullcalendar/react` against `GET /api/calendar/events` (or equivalent).
- Wire the background-image upload flow (`POST`/`GET /api/calendar/backgrounds`, see [BackgroundImage.md](BackgroundImage.md)) with a SignalR listener for `calendar-background-ready` and a Motion crossfade transition.
- Decide Gateway routing vs. separate hosting for the SPA.
