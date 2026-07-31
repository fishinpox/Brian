import { getValidAccessToken, logout } from './auth';

export class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message);
  }
}

export async function apiFetch(baseUrl: string, path: string, init: RequestInit = {}): Promise<Response> {
  const token = await getValidAccessToken();

  const res = await fetch(baseUrl + path, {
    ...init,
    headers: {
      ...init.headers,
      Authorization: `Bearer ${token}`,
    },
  });

  if (res.status === 401) {
    logout();
    throw new ApiError(401, 'Unauthorized');
  }

  if (!res.ok) {
    throw new ApiError(res.status, await res.text());
  }

  return res;
}

export async function apiFetchJson<T>(baseUrl: string, path: string, init: RequestInit = {}): Promise<T> {
  const res = await apiFetch(baseUrl, path, init);
  return (await res.json()) as T;
}

export function withJsonBody(body: unknown): RequestInit {
  return {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  };
}
