export const IDENTITY_API = 'https://localhost:7001';
export const CALENDAR_API = 'https://localhost:7002';
export const HOLODEX_API = 'https://localhost:7009';
export const YOUTUBE_API = 'https://localhost:7011';
export const NOTIFICATIONS_API = 'https://localhost:7008';
export const CHAT_API = 'https://localhost:7013';
export const ONBOARDING_LOGIN_URL = 'https://localhost:7012/login.html';
/** Self-hosted Stoat's own web client, embedded directly for guaranteed Discord-parity UX. */
export const STOAT_WEB_URL = 'http://localhost:8880';

export interface PlatformConfig {
  key: 'holodex' | 'youtube';
  label: string;
  /** Base URL for search/browse. Always Holodex's, even for the youtube platform -
   *  VTuber discovery goes through Holodex's directory since it already maps to
   *  YouTube channel IDs (see the original youtube-follow.html's own comment). */
  searchBaseUrl: string;
  searchEndpoint: string;
  /** Base URL + endpoint for the platform-specific followed-channels list. */
  followedBaseUrl: string;
  followedEndpoint: string;
  statusEndpoint: string;
  credentialEndpoint: string;
  /** Shown on the Calendar page's connection groupbox, above the Connect button. */
  groupboxHint?: string;
  /** Shown on the follow-picker page, above the search box. */
  followPageHint?: string;
  /** Shown if the initial browse-list fetch fails; youtube-follow.html only checked this explicitly. */
  browseFailureMessage?: string;
  searchErrorMessage: string;
}

export const PLATFORM_CONFIGS: Record<'holodex' | 'youtube', PlatformConfig> = {
  holodex: {
    key: 'holodex',
    label: 'Holodex',
    searchBaseUrl: HOLODEX_API,
    searchEndpoint: '/api/holodex/search',
    followedBaseUrl: HOLODEX_API,
    followedEndpoint: '/api/holodex/followed',
    statusEndpoint: '/api/holodex/status',
    credentialEndpoint: '/api/holodex/credential',
    searchErrorMessage: 'Could not search Holodex. Please try again.',
  },
  youtube: {
    key: 'youtube',
    label: 'YouTube',
    searchBaseUrl: HOLODEX_API,
    searchEndpoint: '/api/holodex/search',
    followedBaseUrl: YOUTUBE_API,
    followedEndpoint: '/api/youtube/followed',
    statusEndpoint: '/api/youtube/status',
    credentialEndpoint: '/api/youtube/credential',
    groupboxHint:
      "Finding and following VTubers uses Holodex's directory and doesn't need this. Connecting a YouTube Data API key now gets it ready for tracking subscriber growth and who's live, so we can help surface collaborations across the VTubers you follow.",
    followPageHint:
      "Streamers are discovered through Holodex's free VTuber directory, since it already maps to YouTube channel IDs. Connect Holodex on the Calendar page first if search comes up empty.",
    browseFailureMessage:
      'Could not browse Holodex. Connect Holodex on the Calendar page first, then search by name above.',
    searchErrorMessage: 'Could not search Holodex. Connect Holodex on the Calendar page first.',
  },
};
