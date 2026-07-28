import { useQuery } from '@tanstack/react-query';
import { apiFetchJson } from '../apiClient';
import { CALENDAR_API } from '../../config/platforms';

interface PersonalEvent {
  title: string;
  startAt: string;
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
  title: string;
  when: string;
  kind: 'personal' | 'live' | 'upcoming';
  platform?: string;
}

function toCalendarItems(data: CalendarResponse): CalendarItem[] {
  const items: CalendarItem[] = [
    ...data.personalEvents.map((e, i) => ({
      id: `personal-${i}-${e.startAt}`,
      title: e.title,
      when: e.startAt,
      kind: 'personal' as const,
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
