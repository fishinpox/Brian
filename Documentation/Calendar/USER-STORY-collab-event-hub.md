# User Story — Collab Event Hub

**Feature name:** Collab Event Hub
**Status:** Draft
**Related:** [USER-STORY-nested-folders-calendar.md](USER-STORY-nested-folders-calendar.md) (events live in folders), [MASTER_SPEC.md](MASTER_SPEC.md) §6.1 (Friends & Social — non-negotiable event sharing/group chat), §6.5 (AI Assistant Layer — recommendation angle)

---

## Story

**As a** VTuber setting up a collab,
**I want** a single place tied to a collab event where I can share the event itself, share assets with collaborators, manage who else can invite people, export the event to external calendars, and talk privately with just the people invited,
**so that** collab logistics (who's coming, what assets are shared, what was discussed) live with the event instead of scattered across DMs and file links.

---

## Behavior 1 — Share schedule event to collab

- An event can be shared to one or more collaborators, consistent with the existing non-negotiable friend/event-sharing behavior (MASTER_SPEC §6.1): on invite approval, a group chat is created for the event with file sharing.
- This story treats that group chat + the event itself as the seed of the **Collab Event Hub** — the rest of this story's features attach to that same event.

## Behavior 2 — Per-collab cloud asset folder

- Each collab event gets an attached cloud folder for assets shared specifically with that collab's participants (distinct from a creator's general asset library).
- Every participating streamer's **own asset library** (their general library, not collab-specific) can be attached/shared into this collab folder — so collaborators can pull from each other's existing assets rather than re-uploading.

## Behavior 3 — Invite link propagation

- Approved participants can share the **same invite link** onward to add people not originally associated with the collab (transitive invites), rather than each new invite requiring the organizer's direct action.
- The event supports **export to external calendars** (Google Calendar, Outlook) via standard `.ics` export, so invitees who don't use this platform can still track the collab on their own calendar.

## Behavior 4 — Private per-event chat

- The group chat created on invite approval (Behavior 1) is scoped strictly to invited participants and persists as that collab's saved history — reachable later from the collab event itself, not just from a general chat/messages list.

## Behavior 5 — Recommendation (speculative — not builder-ready)

> **Flag:** this is an AI-assisted recommendation feature (MASTER_SPEC §6.5 AI Assistant Layer). Per MASTER_SPEC §11's AI-specific rules, any such feature must produce *proposed* suggestions only, never auto-populate collab data without confirmation. This needs a product decision on data sourcing (what counts as "historical games played together," where trending-game data comes from) before it's ready to spec in detail — included here as a placeholder, not a committed feature.

- Idea: surface a suggestion of games the collaborators have played together before, or currently-trending games, when setting up a collab event — to speed up planning, not to auto-decide content.

---

## Acceptance criteria

- [ ] Sharing an event to a collaborator creates (or reuses) a group chat scoped to that event's invitees, per MASTER_SPEC §6.1.
- [ ] Each collab event has an attached cloud asset folder, separate from participants' general asset libraries.
- [ ] A participant can attach their own general asset library's contents into the collab's shared folder.
- [ ] An approved participant's invite link, shared onward, successfully adds a new person to the collab who wasn't part of the original invite.
- [ ] The collab event can be exported as a standard `.ics` file importable into Google Calendar and Outlook.
- [ ] The collab's chat history is reachable from the collab event itself later, not only from a general messages list.

## Out of scope
- The game-recommendation feature (Behavior 5) — explicitly flagged as speculative, not part of this story's acceptance criteria.
- Fine-grained per-asset permissions within the shared collab folder (e.g., view-only vs. download) — start with folder-level sharing, refine later if needed.
