# User Profiles & Roles - Calendar App

**Purpose:** This is the single source of truth for all user roles, permissions, account structures, family systems, moderation, and business logic in the platform.

**Last Updated:** July 2026 | **Version:** 1.4 (Final)

---

## Account & Multi-Profile System

- New users sign up via **OAuth** (Google recommended). The system stores the OAuth token and prompts the user to create a hidden **username + password** as backup login.
- Each user can manage **up to 3 profiles** (additional profiles available for purchase).
- Profiles maintain **completely separate data** except for **marketplace store purchases**, which are shared across profiles.
- Users can **switch between profiles seamlessly** without logging out.
- Recommended profile purposes:
  - Profile 1: Family Account
  - Profile 2: Personal / Private Life
  - Profile 3: Public / Professional (Creator, Artist, Business, etc.)

**Cross-Profile Rules:**
- Profiles under the same user **cannot** directly interact with each other (follow, join communities, etc.).
- Only store purchases and explicitly shared calendar events are visible across profiles.

---

## Family Account Structure

- One profile can be designated as the **Family Account**.
- **Head of Family** (set by user) has full visibility and control.
- **Other Parent / Co-Head**
- **Caretaker** – Requires friendship + explicit permission from a parent. Can view and mark tasks complete.
- **Child** – Strictly restricted:
  - Completely isolated from public events and communities.
  - Can only view assigned family tasks and mark them complete.
  - No access to marketplace, payments, storefronts, or external communities.
- Either parent can create Child accounts. Adding Caretakers or external invites requires approval from both parents (Head of Family has final say).

---

## Role Types

### 1. Platform Roles (Internal Only)
- **Platform Admin / Super Admin** – Restricted to developers and internal software staff only.
- **Support Staff**

### 2. Account Roles (Customer-Facing)
- **Fan** (Default)
- **Creator** (Streamer + Content Creator)
- **Artist**
- **Business / Corporate Account**
- **Agency Owner** (Highest CRM purchaser tier – CEO / primary purchaser)

### 3. Community / Assignable Roles
Assignable by Creators, Artists, Businesses, and Agency Owners.

### 4. Recognition Tiers & Global Roles
- Customizable membership tiers (Patreon-style)
- Global Roles (e.g. Verified Artist) – platform-wide recognition

---

## Permissions Overview

- Every user has a **Personal Calendar** and can **publicly share calendar events**.
- All users can join multiple communities with different membership tiers simultaneously.
- **Private Profile Mode** is available to hide activities from public search.

---

## Membership Tiers (Patreon-style)

Creators, Artists, Businesses, and Agency Owners can freely:
- Create and customize unlimited tiers (Free, Tier 1, Tier 2, Tier 3, MVP, etc.).
- Set custom pricing and toggle specific benefits per tier (calendar visibility, private events, badges, etc.).

---

## Global Roles vs Community Roles

- **Global Roles** (e.g. Verified Artist) grant platform-wide recognition, a visible badge, and act as a verifiable point of contact.
- Global Roles provide basic recognition in every community the user joins.
- Community roles are scoped to the granting community.
- Both types can be held simultaneously.

---

## Moderation & Punishment System

- **Suspended**: Permanent public removal with an administrative message visible in chat. Applies platform-wide.
- **Restricted**: Community-scoped punishment. Moderators can choose duration and severity (read-only, no posting, etc.).
- Authoritative accounts (Creator, Artist, Business, Agency Owner) can:
  - View a user’s punishment history.
  - Block users with past punishments from joining their communities (toggleable setting).
- Appeals process: Community Owner → Support Staff → Platform Admin (escalation for persistent cases).
- Any Fan account that receives a suspension, restriction, or ban receives a **permanent mark** on their record.

---

## Assignable Roles

Creators, Artists, Businesses, and Agency Owners can assign the following via submission forms:

- Personal Manager
- Community Manager
- Moderator
- Editor
- Clipper (Shorts / Highlights)
- Writer
- Producer
- Translator
- Artist (Internal)
- Staff / Coordinator
- Guest Collaborator (Temporary)
- Beta Tester / Insider
- Affiliate / Promoter
- Finance / Accountant
- Marketing Manager
- Legal / Compliance
- And other custom roles

**Temporary & Scoped Permissions:**
- Roles can be time-bound or limited to specific projects/dates.
- When a project is marked complete, it is automatically archived (read-only) and moved to an Archive Catalog.

---

## Verification System

- Creators, Artists, and Businesses must complete a verification process to receive official badges.
- Global Roles serve as trust signals for commissions and service requests.

---

## Additional Rules

- **Inactivity**: Accounts inactive for 1 year may be automatically removed.
- **Commissions**: Commissioner must approve display of completed work before it appears publicly on their page (then auto-populates on the Artist’s page).
- **Analytics**: Higher roles (Creator, Artist, Business, Agency Owner) have access to relevant community and revenue analytics.

---

**Implementation Notes**
- Strong data isolation between profiles is required.
- Flexible, community-driven moderation tools.
- Support for users holding multiple roles (Global + Community).
- Family accounts have strict protections, especially for Child roles.
- The system must support users operating across personal, family, and public/professional contexts.

This document serves as the master reference for building user systems, permissions, calendars, communities, moderation, and marketplace features.