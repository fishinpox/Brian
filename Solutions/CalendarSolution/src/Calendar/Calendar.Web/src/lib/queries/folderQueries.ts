import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiFetch, apiFetchJson, withJsonBody } from '../apiClient';
import { CALENDAR_API } from '../../config/platforms';

export interface FolderedEvent {
  id: string;
  title: string;
  startAt: string;
  isVisible: boolean;
  isDraggable: boolean;
}

export interface Subfolder {
  id: string;
  folderId: string;
  name: string;
  isVisible: boolean;
  isLocked: boolean;
  events: FolderedEvent[];
}

export interface Folder {
  id: string;
  name: string;
  colorBackground: string;
  colorText: string;
  colorBorder: string;
  isVisible: boolean;
  isLocked: boolean;
  subfolders: Subfolder[];
}

export interface FolderTree {
  masterLockEnabled: boolean;
  folders: Folder[];
}

export interface CalendarSettings {
  masterLockEnabled: boolean;
  lockFoldersByDefault: boolean;
  autoRecolorByTimeSensitivity: boolean;
  dueSoonWindowHours: number;
}

const FOLDER_TREE_KEY = ['folder-tree'];
const CALENDAR_SETTINGS_KEY = ['calendar-settings'];

function putJson(body: unknown): RequestInit {
  return { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) };
}

export function useFolderTree() {
  return useQuery({
    queryKey: FOLDER_TREE_KEY,
    queryFn: () => apiFetchJson<FolderTree>(CALENDAR_API, '/api/calendar/folders'),
  });
}

export function useCreateFolder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (name: string) => apiFetchJson<Folder>(CALENDAR_API, '/api/calendar/folders', withJsonBody({ name })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useRenameFolder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ folderId, name }: { folderId: string; name: string }) =>
      apiFetch(CALENDAR_API, `/api/calendar/folders/${folderId}`, putJson({ name })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useSetFolderColor() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ folderId, background, text, border }: { folderId: string; background: string; text: string; border: string }) =>
      apiFetch(CALENDAR_API, `/api/calendar/folders/${folderId}/color`, putJson({ background, text, border })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useSetFolderVisibility() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ folderId, isVisible }: { folderId: string; isVisible: boolean }) =>
      apiFetch(CALENDAR_API, `/api/calendar/folders/${folderId}/visibility`, putJson({ isVisible })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useSetFolderLock() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ folderId, isLocked }: { folderId: string; isLocked: boolean }) =>
      apiFetch(CALENDAR_API, `/api/calendar/folders/${folderId}/lock`, putJson({ isLocked })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useDeleteFolder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (folderId: string) => apiFetch(CALENDAR_API, `/api/calendar/folders/${folderId}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useCreateSubfolder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ folderId, name }: { folderId: string; name: string }) =>
      apiFetchJson<Subfolder>(CALENDAR_API, `/api/calendar/folders/${folderId}/subfolders`, withJsonBody({ name })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useRenameSubfolder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ subfolderId, name }: { subfolderId: string; name: string }) =>
      apiFetch(CALENDAR_API, `/api/calendar/subfolders/${subfolderId}`, putJson({ name })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useSetSubfolderVisibility() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ subfolderId, isVisible }: { subfolderId: string; isVisible: boolean }) =>
      apiFetch(CALENDAR_API, `/api/calendar/subfolders/${subfolderId}/visibility`, putJson({ isVisible })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useSetSubfolderLock() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ subfolderId, isLocked }: { subfolderId: string; isLocked: boolean }) =>
      apiFetch(CALENDAR_API, `/api/calendar/subfolders/${subfolderId}/lock`, putJson({ isLocked })),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useDeleteSubfolder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (subfolderId: string) => apiFetch(CALENDAR_API, `/api/calendar/subfolders/${subfolderId}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY }),
  });
}

export function useCalendarSettings() {
  return useQuery({
    queryKey: CALENDAR_SETTINGS_KEY,
    queryFn: () => apiFetchJson<CalendarSettings>(CALENDAR_API, '/api/calendar/settings'),
  });
}

export function useUpdateCalendarSettings() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (settings: CalendarSettings) =>
      apiFetchJson<CalendarSettings>(CALENDAR_API, '/api/calendar/settings', putJson(settings)),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CALENDAR_SETTINGS_KEY });
      queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY });
      queryClient.invalidateQueries({ queryKey: ['calendar-events'] });
    },
  });
}

export function useSetEventVisibility() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ eventId, isVisible }: { eventId: string; isVisible: boolean }) =>
      apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}/visibility`, putJson({ isVisible })),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY });
      queryClient.invalidateQueries({ queryKey: ['calendar-events'] });
    },
  });
}

export function useMoveEventToFolder() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ eventId, subfolderId }: { eventId: string; subfolderId: string | null }) =>
      apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}/folder`, putJson({ subfolderId })),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY });
      queryClient.invalidateQueries({ queryKey: ['calendar-events'] });
    },
  });
}

export function useMoveEventDate() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ eventId, newStartAt }: { eventId: string; newStartAt: string }) =>
      apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}/date`, putJson({ newStartAt })),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: FOLDER_TREE_KEY });
      queryClient.invalidateQueries({ queryKey: ['calendar-events'] });
    },
  });
}
