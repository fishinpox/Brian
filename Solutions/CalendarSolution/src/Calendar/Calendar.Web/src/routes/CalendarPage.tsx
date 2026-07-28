import { useMemo } from 'react';
import FullCalendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';
import type { EventInput } from '@fullcalendar/core';
import { useCalendarEvents } from '../lib/queries/calendarQueries';
import { EventList } from '../components/EventList';
import { ConnectionGroupbox } from '../components/ConnectionGroupbox';
import { PLATFORM_CONFIGS } from '../config/platforms';
import { decodeJwt, getToken, logout } from '../lib/auth';

const KIND_COLORS: Record<string, string> = {
  personal: '#1a73e8',
  live: '#d93025',
  upcoming: '#b06000',
};

export function CalendarPage() {
  const { from, to } = useMemo(() => {
    const start = new Date();
    start.setHours(0, 0, 0, 0);
    const end = new Date(start);
    end.setDate(end.getDate() + 30);
    return { from: start, to: end };
  }, []);

  const events = useCalendarEvents(from, to);
  const token = getToken();
  const claims = token ? decodeJwt(token) : null;

  const fullCalendarEvents: EventInput[] = (events.data ?? []).map((item) => ({
    id: item.id,
    title: item.title,
    start: item.when,
    color: KIND_COLORS[item.kind],
  }));

  return (
    <div className="mx-auto max-w-3xl px-6 py-10">
      <header className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold text-gray-800">Brian</h1>
          {claims?.unique_name ? (
            <p className="mt-0.5 text-xs text-gray-500">Welcome, @{String(claims.unique_name)}</p>
          ) : null}
        </div>
        <button
          type="button"
          onClick={logout}
          className="rounded-md border border-gray-300 px-4 py-2 text-xs text-gray-500 transition hover:bg-gray-100"
        >
          Sign Out
        </button>
      </header>

      <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
        <FullCalendar
          plugins={[dayGridPlugin, timeGridPlugin, listPlugin]}
          initialView="listMonth"
          headerToolbar={{
            left: 'prev,next today',
            center: 'title',
            right: 'listMonth,dayGridMonth,timeGridWeek',
          }}
          events={fullCalendarEvents}
          height="auto"
        />
      </div>

      <div className="mt-6 rounded-lg bg-white p-6 shadow-sm">
        <h2 className="mb-4 text-base font-semibold text-gray-800">Upcoming</h2>
        <EventList items={events.data ?? []} isLoading={events.isLoading} isError={events.isError} />
      </div>

      <ConnectionGroupbox config={PLATFORM_CONFIGS.holodex} />
      <ConnectionGroupbox config={PLATFORM_CONFIGS.youtube} />
    </div>
  );
}
