import { useQuery } from '@tanstack/react-query';
import { apiFetchJson } from '../apiClient';
import { CHAT_API } from '../../config/platforms';

export interface ChatAccount {
  username: string;
  email: string;
  password: string;
}

/** Idempotent - safe to call every time the chat panel opens, not just the first time. */
export function useChatAccount(enabled: boolean) {
  return useQuery({
    queryKey: ['chat-account'],
    queryFn: () => apiFetchJson<ChatAccount>(CHAT_API, '/api/chat/account', { method: 'POST' }),
    enabled,
    staleTime: Infinity,
  });
}
