import { IDENTITY_API, ONBOARDING_LOGIN_URL } from '../config/platforms';

const TOKEN_KEY = 'token';
const REFRESH_TOKEN_KEY = 'refreshToken';
const EXPIRY_BUFFER_MS = 30_000;

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function getRefreshToken(): string | null {
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

function setTokens(token: string, refreshToken: string): void {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
}

/** Reads `?token=`/`?refreshToken=` query params on first load, persists them, and strips them from the URL. */
export function captureTokenFromUrl(): void {
  const params = new URLSearchParams(window.location.search);
  const queryToken = params.get('token');
  const queryRefreshToken = params.get('refreshToken');

  if (queryToken) {
    localStorage.setItem(TOKEN_KEY, queryToken);
    params.delete('token');
    if (queryRefreshToken) {
      localStorage.setItem(REFRESH_TOKEN_KEY, queryRefreshToken);
      params.delete('refreshToken');
    }
    const rest = params.toString();
    window.history.replaceState({}, '', window.location.pathname + (rest ? `?${rest}` : ''));
  }
}

export function redirectToLogin(): void {
  window.location.href = ONBOARDING_LOGIN_URL;
}

export function logout(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  redirectToLogin();
}

interface JwtClaims {
  unique_name?: string;
  exp?: number;
  [key: string]: unknown;
}

export function decodeJwt(token: string): JwtClaims | null {
  try {
    const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const padded = payload + '='.repeat((4 - (payload.length % 4)) % 4);
    return JSON.parse(atob(padded)) as JwtClaims;
  } catch {
    return null;
  }
}

function isExpiringSoon(token: string): boolean {
  const claims = decodeJwt(token);
  if (!claims?.exp) return true;
  return claims.exp * 1000 <= Date.now() + EXPIRY_BUFFER_MS;
}

interface RefreshResponse {
  token: string;
  refreshToken: string;
}

let refreshInFlight: Promise<string | null> | null = null;

/**
 * Returns a currently-valid access token, silently refreshing first if it's expired or
 * about to be. Used by both apiClient's REST calls and signalr's accessTokenFactory so a
 * mid-session refresh is visible to an already-open hub connection rather than a stale
 * capture - same approach as NativeApp.Core's AuthSession.
 */
export async function getValidAccessToken(): Promise<string | null> {
  const token = getToken();
  if (!token) return null;

  if (!isExpiringSoon(token)) return token;

  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    logout();
    return null;
  }

  refreshInFlight ??= (async () => {
    try {
      const res = await fetch(`${IDENTITY_API}/api/auth/refresh`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken }),
      });

      if (!res.ok) {
        logout();
        return null;
      }

      const data = (await res.json()) as RefreshResponse;
      setTokens(data.token, data.refreshToken);
      return data.token;
    } catch {
      logout();
      return null;
    } finally {
      refreshInFlight = null;
    }
  })();

  return refreshInFlight;
}
