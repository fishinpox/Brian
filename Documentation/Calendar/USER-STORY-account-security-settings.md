# User Story — Account Security Settings

**Feature name:** Account Security Settings
**Status:** Draft
**Related:** [MASTER_SPEC.md](MASTER_SPEC.md) §11 (Security Requirements — the baseline this UI surfaces), [USER-STORY-team-sharing.md](USER-STORY-team-sharing.md)

---

## Story

**As a** creator who's logged in on multiple devices,
**I want** to see which devices are currently signed into my account and log any of them out remotely, with confidence my data is synced and backed up wherever I log in,
**so that** I can catch and shut down access from a device I no longer use or don't recognize.

---

## Layout & Behavior

### Online device list
- Settings → "Account Security" (or similar) shows a list of devices currently signed into the account: device/browser identifier, approximate location/last-active time, and a **log out** action per device.
- Logging out a device immediately invalidates its session — consistent with MASTER_SPEC §11's baseline (OAuth tokens encrypted at rest, authorization checks on every route).

### Sync confirmation
- A simple confirmation that the account syncs in real time across devices (no separate action needed — logging in on a new device shows the same data immediately), surfaced as informational text rather than a control, since this is inherent to the SignalR-backed architecture (MASTER_SPEC §5.1/§6.4), not a togglable feature.

---

## Acceptance criteria

- [ ] The Account Security settings page lists all devices with an active session for the account.
- [ ] Logging out a listed device invalidates that device's session; the device is prompted to re-authenticate on its next action.
- [ ] The currently-in-use device is clearly distinguished from other listed devices (e.g., "this device" label).
- [ ] Signing into a new device surfaces the account's current data immediately, without a manual sync step.

## Out of scope
- 2FA setup/management UI — MASTER_SPEC §11 mandates 2FA for creator/partner/venue/manager/agency/admin roles; the enrollment flow itself is a separate, already-implied piece of the account system, not redefined here.
- Session-anomaly alerting (e.g., "new device login from unusual location") — a good candidate for MASTER_SPEC §11's anomaly-alerts baseline, but not specified as a frontend feature in this pass.
