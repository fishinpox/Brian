import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiFetch, apiFetchJson, withJsonBody } from '../apiClient';
import { CALENDAR_API } from '../../config/platforms';

export interface Note {
  id: string;
  title: string;
  content: string;
  createdAt: string;
  updatedAt: string;
}

const NOTES_KEY = ['notes'];

export function useNotes() {
  return useQuery({
    queryKey: NOTES_KEY,
    queryFn: () => apiFetchJson<Note[]>(CALENDAR_API, '/api/calendar/notes'),
  });
}

export function useCreateNote() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ title, content }: { title: string; content: string }) =>
      apiFetchJson<Note>(CALENDAR_API, '/api/calendar/notes', withJsonBody({ title, content })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: NOTES_KEY }),
  });
}

export function useUpdateNote() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ noteId, title, content }: { noteId: string; title: string; content: string }) =>
      apiFetch(CALENDAR_API, `/api/calendar/notes/${noteId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, content }),
      }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: NOTES_KEY }),
  });
}

export function useDeleteNote() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (noteId: string) => apiFetch(CALENDAR_API, `/api/calendar/notes/${noteId}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: NOTES_KEY }),
  });
}
