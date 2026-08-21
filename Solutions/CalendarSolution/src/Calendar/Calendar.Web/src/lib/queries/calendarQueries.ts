import { useQuery } from '@tanstack/react-query';
import { apiFetchJson } from '../apiClient';
import { CALENDAR_API } from '../../config/platforms';

export type CountdownCategory = 'Birthday' | 'Holiday' | 'Anniversary' | 'Exam' | 'Default';
export type RecurrenceType = 'None' | 'Daily' | 'Weekly' | 'Monthly';

export interface PersonalEvent {
  id: string;
  title: string;
  description: string | null;
  location: string | null;
  startAt: string;
  endAt: string | null;
  isAllDay: boolean;
  subfolderId: string | null;
  isVisible: boolean;
  isDraggable: boolean;
  isCompleted: boolean;
  countdownCategory: CountdownCategory | null;
  recurrenceType: RecurrenceType;
  recurrenceEndDate: string | null;
  autoDeferEnabled: boolean;
}

interface StreamEvent {
  title: string;
  scheduledStart: string;
  isLive: boolean;
  platform: string;
}

interface CalendarResponse {
  personalEvents: PersonalEvent[];
  streamEvents: StreamEvent[];
}

export interface CalendarItem {
  id: string;
  eventId?: string;
  title: string;
  when: string;
  kind: 'personal' | 'live' | 'upcoming';
  platform?: string;
  personalEvent?: PersonalEvent;
}

function toCalendarItems(data: CalendarResponse): CalendarItem[] {
  const items: CalendarItem[] = [
    ...data.personalEvents.map((e) => ({
      id: `personal-${e.id}`,
      eventId: e.id,
      title: e.title,
      when: e.startAt,
      kind: 'personal' as const,
      personalEvent: e,
    })),
    ...data.streamEvents.map((e, i) => ({
      id: `stream-${i}-${e.scheduledStart}`,
      title: e.title,
      when: e.scheduledStart,
      kind: e.isLive ? ('live' as const) : ('upcoming' as const),
      platform: e.platform,
    })),
  ];

  return items.sort((a, b) => new Date(a.when).getTime() - new Date(b.when).getTime());
}

export function useCalendarEvents(from: Date, to: Date) {
  return useQuery({
    queryKey: ['calendar-events', from.toISOString(), to.toISOString()],
    queryFn: async () => {
      const data = await apiFetchJson<CalendarResponse>(
        CALENDAR_API,
        `/api/calendar?from=${from.toISOString()}&to=${to.toISOString()}`
      );
      return toCalendarItems(data);
    },
  });
}
