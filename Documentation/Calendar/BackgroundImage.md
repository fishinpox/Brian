# Calendar Background Image

**Purpose:** Allow a user to upload a custom background image for their Personal Calendar.

**Last Updated:** July 2026 | **Version:** 1.0

---

## Goal

Users can upload an image of their own choosing to personalize their calendar background.

## Guidelines (from original requirements)

- The image must be scanned for CSAM, virus, and malware signatures before it goes live. **Deferred** — see Implementation Notes.
- The image may only be of certain types (`.png`, `.jpeg`).
- The image size is limited to 10 MB.
- The image is saved in a table associated with the user.
- When the user submits the image, the client shows a "Image submitted!" popup.
- Once the image is approved and saved, a notification is sent to the client to refresh.
- If the image is rejected, the user is emailed the reason. **Deferred** — see Implementation Notes.

## Technical Requirements (from original requirements)

- This runs as a separate service.
- It's queue-based, not real-time — submission, validation, and approval happen asynchronously.

---

## Architecture

Three services participate, communicating over RabbitMQ (MassTransit), matching the rest of the platform's event-driven conventions.

```
Calendar.API                Moderation.API              Notifications.API
  PUT /api/calendar/background   (new service)
    -> save row (Pending)
    -> publish CalendarBackgroundSubmittedEvent
                             -> consume Submitted
                             -> validate type/size/signature
                             -> publish Approved or Rejected
  consume Approved/Rejected                              consume Approved
    -> update row status                                   -> SignalR push
                                                             "calendar-background-ready"
```

### Calendar service (owns the data)

- New entity `CalendarBackground` (one row per profile, unique index on `ProfileId`):
  `Id`, `ProfileId`, `ImageData` (`varbinary(max)`), `ContentType`, `FileName`, `SizeBytes`, `Status` (`Pending`/`Approved`/`Rejected`), `RejectionReason`.
- `PUT /api/calendar/background` — multipart upload, `[Authorize]`. Upserts the row as `Pending`, publishes `CalendarBackgroundSubmittedEvent`, returns `202 Accepted` immediately (client shows "Image submitted!" on this response — it doesn't wait for the async review).
- `GET /api/calendar/background` — streams bytes only when `Status == Approved`; `404` otherwise (client falls back to a default background).
- Consumes `CalendarBackgroundApprovedEvent` / `CalendarBackgroundRejectedEvent` to update the row's status (and `RejectionReason`).

### Moderation service (new, `Solutions/ModerationSolution`)

- Stateless queue consumer, no database of its own.
- Consumes `CalendarBackgroundSubmittedEvent`.
- Validates: content type is `image/png` or `image/jpeg`, size ≤ 10 MB, and the file's magic-byte signature actually matches the declared content type.
- Publishes `CalendarBackgroundApprovedEvent` or `CalendarBackgroundRejectedEvent(..., Reason)`.
- **`Moderation:AutoApprove` (appsettings, defaults to `true`)** — while on, every submission is approved immediately and the type/size/signature check above is skipped entirely, so the end-to-end pipeline can be exercised without fighting validation. Set it to `false` once the type/size/signature check (or real scanning) should actually gate approval.

### Notifications service

- Consumes `CalendarBackgroundApprovedEvent` and pushes `calendar-background-ready` over the existing SignalR hub (`/hubs/notifications`, grouped by `profile_id`) so the client knows to refresh.

---

## Implementation Notes / Deferred Work

- **CSAM / malware scanning is not implemented.** Real detection requires vendor integration (e.g. Microsoft PhotoDNA, Thorn Safer, or NCMEC hash-matching) that needs legal registration this repo doesn't have. The Moderation service's validation step (type/size/signature) is a placeholder for where a real scanner would plug in — flagged as a follow-up, not silently skipped.
- **Rejection email is not implemented.** Notifications service currently has no database and no way to resolve a profile's email address (it only consumes `AccountRegisteredEvent`/`ProfileCreatedEvent` today via nothing — those aren't consumed anywhere yet). Building that lookup is a separate task. For now, a rejected submission is recorded on the `CalendarBackground` row with its `RejectionReason`, and the Calendar-side consumer logs it.
