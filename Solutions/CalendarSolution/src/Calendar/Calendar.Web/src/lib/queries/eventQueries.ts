import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiFetch, apiFetchJson, withJsonBody } from '../apiClient';
import { CALENDAR_API } from '../../config/platforms';
import type { CountdownCategory, PersonalEvent, RecurrenceType } from './calendarQueries';

function putJson(body: unknown): RequestInit {
  return { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) };
}

function invalidateEvents(queryClient: ReturnType<typeof useQueryClient>) {
  queryClient.invalidateQueries({ queryKey: ['calendar-events'] });
  queryClient.invalidateQueries({ queryKey: ['folder-tree'] });
}

export interface CreateEventInput {
  title: string;
  description: string | null;
  location: string | null;
  startAt: string;
  endAt: string | null;
  isAllDay: boolean;
  recurrenceRule: string | null;
}

export function useCreateEvent() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: CreateEventInput) =>
      apiFetchJson<string>(CALENDAR_API, '/api/calendar/events', withJsonBody(input)),
    onSuccess: () => invalidateEvents(queryClient),
  });
}

export function useUpdateEvent() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ eventId, ...input }: CreateEventInput & { eventId: string }) =>
      apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}`, putJson(input)),
    onSuccess: () => invalidateEvents(queryClient),
  });
}

export function useDeleteEvent() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (eventId: string) => apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}`, { method: 'DELETE' }),
    onSuccess: () => invalidateEvents(queryClient),
  });
}

export function useSetEventCompletion() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ eventId, isCompleted }: { eventId: string; isCompleted: boolean }) =>
      apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}/completion`, putJson({ isCompleted })),
    onSuccess: () => invalidateEvents(queryClient),
  });
}

export function useSetEventCountdownCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ eventId, category }: { eventId: string; category: CountdownCategory | null }) =>
      apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}/countdown`, putJson({ category })),
    onSuccess: () => invalidateEvents(queryClient),
  });
}

export function useSetEventRecurrence() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ eventId, recurrenceType, recurrenceEndDate }: { eventId: string; recurrenceType: RecurrenceType; recurrenceEndDate: string | null }) =>
      apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}/recurrence`, putJson({ recurrenceType, recurrenceEndDate })),
    onSuccess: () => invalidateEvents(queryClient),
  });
}

export function useSetEventAutoDefer() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ eventId, autoDeferEnabled }: { eventId: string; autoDeferEnabled: boolean }) =>
      apiFetch(CALENDAR_API, `/api/calendar/events/${eventId}/auto-defer`, putJson({ autoDeferEnabled })),
    onSuccess: () => invalidateEvents(queryClient),
  });
}

export interface CreateReminderPlanInput {
  eventId: string;
  firstTriggerAt: string;
  recurrenceType: RecurrenceType;
  endDate: string | null;
  maxOccurrences: number;
  timesOfDay: string[];
  method: 'Email' | 'Push' | 'InApp';
}

export function useCreateReminderPlan() {
  return useMutation({
    mutationFn: ({ eventId, ...input }: CreateReminderPlanInput) =>
      apiFetchJson<string[]>(CALENDAR_API, `/api/calendar/events/${eventId}/reminder-plan`, withJsonBody(input)),
  });
}

export function useSearchEvents(keyword: string) {
  return useQuery({
    queryKey: ['event-search', keyword],
    queryFn: () => apiFetchJson<PersonalEvent[]>(CALENDAR_API, `/api/calendar/events/search?keyword=${encodeURIComponent(keyword)}`),
    enabled: keyword.trim().length >= 2,
  });
}
