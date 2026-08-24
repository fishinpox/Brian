import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiFetch, ApiError } from '../apiClient';
import { CALENDAR_API } from '../../config/platforms';

export const BACKGROUND_KEY = ['calendar-background'];

export function useCalendarBackground() {
  return useQuery({
    queryKey: BACKGROUND_KEY,
    queryFn: async () => {
      try {
        const res = await apiFetch(CALENDAR_API, '/api/calendar/background');
        const blob = await res.blob();
        return URL.createObjectURL(blob);
      } catch (err) {
        if (err instanceof ApiError && err.status === 404) return null;
        throw err;
      }
    },
  });
}

export function useUploadCalendarBackground() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (file: File) => {
      const formData = new FormData();
      formData.append('file', file);
      return apiFetch(CALENDAR_API, '/api/calendar/background', { method: 'PUT', body: formData });
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: BACKGROUND_KEY }),
  });
}
