import { ONBOARDING_LOGIN_URL } from '../config/platforms';

const TOKEN_KEY = 'token';

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

/** Reads a `?token=` query param on first load, persists it, and strips it from the URL. */
export function captureTokenFromUrl(): void {
  const params = new URLSearchParams(window.location.search);
  const queryToken = params.get('token');

  if (queryToken) {
    localStorage.setItem(TOKEN_KEY, queryToken);
    params.delete('token');
    const rest = params.toString();
    window.history.replaceState({}, '', window.location.pathname + (rest ? `?${rest}` : ''));
  }
}

export function redirectToLogin(): void {
  window.location.href = ONBOARDING_LOGIN_URL;
}

export function logout(): void {
  localStorage.removeItem(TOKEN_KEY);
  redirectToLogin();
}

interface JwtClaims {
  unique_name?: string;
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
