Goal: To allow a user to select which streamers they would like to follow on YouTube.

Guidelines:
Similar to the Holodex solution build out the YouTubeSolution. This API will contain all the methods to access YouTube from the CalendarSolution.
On the Calendar.html page add a new group box for YouTube, matching the existing Holodex groupbox pattern (connect/save credential, then a followed-list summary with a "Manage" link).
Create a new youtube-follow.html page that allows the user to select streamers they want to follow.
After the user is finished selecting their VTubers to follow, save them so each time they are listed in the Calendar.html YouTube groupbox.

Decisions (2026-07-23):

1. Credential model: Reuse the existing API-key pattern already scaffolded in YouTube.API (StoreYouTubeCredentialCommand), the same shape as Holodex's credential entry — no new Google OAuth consent flow. The user enters a YouTube Data API key on calendar.html.
   - This key is NOT used by streamer discovery or the follow-list save/load (both go through Holodex — see decision 3). It's collected up front because two planned follow-up features need it: (a) tracking subscriber growth for followed channels, and (b) tracking which followed channels are actively live streaming — both intended to help surface collaborations across the VTubers a user follows. Until those ship, the credential gate on `calendar.html`'s "Manage VTubers" link is ahead of what's actually wired up, and that's intentional (2026-07-24 decision) rather than a bug.

2. Credential encryption: Upgrade YouTube's credential storage from the current Base64 placeholder (StoreYouTubeCredentialCommandHandler) to AES-256-GCM, mirroring Holodex.Infrastructure's ICredentialEncryptionService/AesCredentialEncryptionService. Keyed by its own `Credentials:EncryptionKey` config/user-secret, same convention as Holodex.

3. Streamer discovery: Do NOT search via YouTube Data API's search.list (100 quota units/call against a 10,000/day default quota — too expensive for repeated name searches). Instead, reuse Holodex's already-integrated `/channels` search (HolodexSolution's IHolodexApiClient.SearchChannelsAsync, free and generously rate-limited). For VTubers who stream natively on YouTube, Holodex's channel `id` is the underlying YouTube channel ID, so a Holodex search result can be used directly as the `channelId` input to YouTube's `IYouTubeApiClient.GetLiveBroadcastsAsync`. youtube-follow.html searches/browses via Holodex, and the selected channel IDs are saved as YouTube FollowedChannel records.
   - Caveat: this surfaces only VTubers Holodex tracks, not arbitrary YouTube channels — acceptable since this product is VTuber-focused and Holodex is the same directory already used for the Holodex follow feature.
   - This means youtube-follow.html depends on the user having a Holodex connection (or at least being able to reach Holodex's search) to discover channels; a channel that streams on YouTube but isn't in Holodex's directory can't be found through this flow.

4. Sync scope: This task covers the follow-list UI and storage only (FollowedChannel entity/config, SaveFollowedChannels/GetFollowedChannels commands, calendar.html groupbox, youtube-follow.html page) — matching Holodex's SaveFollowedChannels/GetFollowedChannels shape. Actually pulling followed channels' live/upcoming streams into Calendar events (the sync pipeline — TriggerYouTubeSync, YouTubeSyncRequestedEvent, a real implementation of the currently-stubbed YouTubeSyncRequestedConsumer) is explicitly out of scope for this task and left for a follow-up.
