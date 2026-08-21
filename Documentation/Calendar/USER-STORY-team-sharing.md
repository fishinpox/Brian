# User Story — Team Sharing UI

**Feature name:** Team Sharing
**Status:** Draft
**Related:** [MASTER_SPEC.md](MASTER_SPEC.md) §4 (Roles & Relationships — the underlying permission model this UI exposes), [USER-STORY-account-security-settings.md](USER-STORY-account-security-settings.md)

---

## Story

**As a** small-team creator (e.g., with a manager or a couple of staff),
**I want** a straightforward frontend flow to invite team members by email, see who's on my team, and trust that each member's own tasks/schedule stay private to them unless explicitly shared,
**so that** I can delegate calendar/task work without needing to understand the full agency-scale permission system underneath.

> **Note:** this is a frontend UI spec for a lightweight team-sharing surface. The underlying permission model (who can do what) is already defined by MASTER_SPEC §4's Manager/Agency roles — this story does not redefine roles, it specifies how a solo creator or small team *accesses* that model through a simple invite-and-view flow, without needing full agency tooling.

---

## Layout & Behavior

### Invite flow
- From Settings → "Team," an **email-based invite**: enter a collaborator's email, they receive an invite, accepting links their account as a team member with permissions scoped per MASTER_SPEC §4 (manager-granted permissions, not a separate ad-hoc permission system).
- A simple team roster view lists current members, their role/permission summary, and an option to revoke access.

### Sub-account / private-schedule visibility
- Each team member's own tasks and personal schedule items are **not shared** with other members by default — only what's explicitly granted (e.g., a shared team calendar, or specific delegated events) is visible across the team.
- This maps directly onto MASTER_SPEC §4's persona/permission model: a manager's granted scope is explicit ("schedule/modify/update... per granted permissions"), not an all-or-nothing team-wide share.

### Team view
- A "Shared Team" view aggregates what each member *has* shared/been granted (delegated events, the shared team calendar) — distinct from each member's private items, which never appear here.

---

## Acceptance criteria

- [ ] A team invite can be sent via email and, once accepted, the invitee appears in the team roster with their granted permission scope visible.
- [ ] Revoking a team member's access removes their granted permissions without deleting their own private account/data.
- [ ] A team member's private tasks/schedule items do not appear in the Shared Team view unless explicitly shared/delegated.
- [ ] The Shared Team view accurately reflects only what has been explicitly granted across members — no accidental over-sharing.

## Out of scope
- Defining new permission granularity beyond what MASTER_SPEC §4 already specifies — this story is a UI layer on top of the existing role model, not a new backend permission scheme.
- Agency-scale team management (contracts, onboarding, training modules — MASTER_SPEC §5.7 Agency tier) — this story is scoped to the lightweight "small team, no agency required" case per MASTER_SPEC §5.7's "Manager mode."
