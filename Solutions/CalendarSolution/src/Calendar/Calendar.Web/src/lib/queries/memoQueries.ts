import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiFetch, apiFetchJson, withJsonBody } from '../apiClient';
import { CALENDAR_API } from '../../config/platforms';

export interface Memo {
  id: string;
  date: string;
  text: string;
  createdAt: string;
}

export function useMemos(from: string, to: string) {
  return useQuery({
    queryKey: ['memos', from, to],
    queryFn: () => apiFetchJson<Memo[]>(CALENDAR_API, `/api/calendar/memos?from=${from}&to=${to}`),
  });
}

export function useCreateMemo() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ date, text }: { date: string; text: string }) =>
      apiFetchJson<Memo>(CALENDAR_API, '/api/calendar/memos', withJsonBody({ date, text })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['memos'] }),
  });
}

export function useDeleteMemo() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (memoId: string) => apiFetch(CALENDAR_API, `/api/calendar/memos/${memoId}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['memos'] }),
  });
}
