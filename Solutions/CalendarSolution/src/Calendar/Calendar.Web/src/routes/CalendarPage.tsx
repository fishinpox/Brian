import { useMemo, useRef, useState, type DragEvent as ReactDragEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import FullCalendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';
import type { EventInput } from '@fullcalendar/core';
import { AnimatePresence, motion } from 'motion/react';
import { useCalendarEvents, type CalendarItem, type CountdownCategory, type PersonalEvent } from '../lib/queries/calendarQueries';
import { useCalendarSettings, useFolderTree, useMoveEventDate, type Folder } from '../lib/queries/folderQueries';
import { useMemos } from '../lib/queries/memoQueries';
import { EventList } from '../components/EventList';
import { ConnectionGroupbox } from '../components/ConnectionGroupbox';
import { FolderTree } from '../components/FolderTree';
import { EventEditBox } from '../components/EventEditBox';
import { PluginMenu } from '../components/PluginMenu';
import { PLATFORM_CONFIGS } from '../config/platforms';
import { decodeJwt, getToken, logout } from '../lib/auth';

const KIND_COLORS: Record<string, string> = {
  personal: '#1a73e8',
  live: '#d93025',
  upcoming: '#b06000',
};

const DUE_SOON_COLOR = '#FF9800';
const OVERDUE_COLOR = '#F44336';

const COUNTDOWN_COLORS: Record<CountdownCategory, { background: string; border: string; text: string }> = {
  Birthday: { background: '#FCE4EC', border: '#C2185B', text: '#C2185B' },
  Holiday: { background: '#E8F5E9', border: '#388E3C', text: '#388E3C' },
  Anniversary: { background: '#F3E5F5', border: '#7B1FA2', text: '#7B1FA2' },
  Exam: { background: '#FFEBEE', border: '#C62828', text: '#C62828' },
  Default: { background: '#F5F5F5', border: '#616161', text: '#616161' },
};

function monthLabel(date: Date): string {
  return date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
}

function toDateOnly(d: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

export function CalendarPage() {
  const { from, to } = useMemo(() => {
    const start = new Date();
    start.setHours(0, 0, 0, 0);
    const end = new Date(start);
    end.setDate(end.getDate() + 30);
    return { from: start, to: end };
  }, []);

  const events = useCalendarEvents(from, to);
  const memos = useMemos(toDateOnly(from), toDateOnly(to));
  const folderTree = useFolderTree();
  const settings = useCalendarSettings();
  const moveEventDate = useMoveEventDate();

  const token = getToken();
  const claims = token ? decodeJwt(token) : null;
  // A single-role JWT serializes `roles` as a bare string rather than a one-element array.
  const rawRoles = claims?.roles;
  const roles = Array.isArray(rawRoles) ? (rawRoles as string[]) : typeof rawRoles === 'string' ? [rawRoles] : [];
  const isCreator = roles.includes('Creator');
  const navigate = useNavigate();

  const calendarRef = useRef<FullCalendar>(null);
  const [viewStart, setViewStart] = useState(new Date());
  const [toast, setToast] = useState<string | null>(null);
  const toastTimer = useRef<number | undefined>(undefined);
  const [editorState, setEditorState] = useState<{ date: Date; eventId?: string } | null>(null);

  function showToast(message: string) {
    setToast(message);
    window.clearTimeout(toastTimer.current);
    toastTimer.current = window.setTimeout(() => setToast(null), 2500);
  }

  const subfolderToFolder = useMemo(() => {
    const map = new Map<string, { folder: Folder; isVisible: boolean }>();
    for (const folder of folderTree.data?.folders ?? []) {
      for (const subfolder of folder.subfolders) {
        map.set(subfolder.id, { folder, isVisible: subfolder.isVisible });
      }
    }
    return map;
  }, [folderTree.data]);

  const eventById = useMemo(() => {
    const map = new Map<string, PersonalEvent>();
    for (const item of events.data ?? []) {
      if (item.personalEvent) map.set(item.personalEvent.id, item.personalEvent);
    }
    return map;
  }, [events.data]);

  const memosByDate = useMemo(() => {
    const map = new Map<string, string[]>();
    for (const memo of memos.data ?? []) {
      const list = map.get(memo.date) ?? [];
      list.push(memo.text);
      map.set(memo.date, list);
    }
    return map;
  }, [memos.data]);

  function isEffectivelyVisible(item: CalendarItem): boolean {
    if (item.kind !== 'personal' || !item.personalEvent) return true;
    if (!item.personalEvent.isVisible) return false;
    const subfolderId = item.personalEvent.subfolderId;
    if (!subfolderId) return true;
    const entry = subfolderToFolder.get(subfolderId);
    if (!entry) return true;
    return entry.isVisible && entry.folder.isVisible;
  }

  function colorsForItem(item: CalendarItem): { background: string; border: string; text?: string } {
    const fallback = KIND_COLORS[item.kind] ?? KIND_COLORS.personal;
    if (item.kind !== 'personal' || !item.personalEvent) {
      return { background: fallback, border: fallback };
    }

    // Countdown category styling takes precedence over folder-membership color.
    if (item.personalEvent.countdownCategory) {
      return COUNTDOWN_COLORS[item.personalEvent.countdownCategory];
    }

    const subfolderId = item.personalEvent.subfolderId;
    if (!subfolderId) return { background: fallback, border: fallback };

    const entry = subfolderToFolder.get(subfolderId);
    if (!entry) return { background: fallback, border: fallback };

    if (settings.data?.autoRecolorByTimeSensitivity) {
      const hoursUntilDue = (new Date(item.when).getTime() - Date.now()) / 3_600_000;
      if (hoursUntilDue < 0) return { background: OVERDUE_COLOR, border: OVERDUE_COLOR, text: '#ffffff' };
      if (hoursUntilDue <= settings.data.dueSoonWindowHours) {
        return { background: DUE_SOON_COLOR, border: DUE_SOON_COLOR, text: '#ffffff' };
      }
    }

    return { background: entry.folder.colorBackground, border: entry.folder.colorBorder, text: entry.folder.colorText };
  }

  const visibleItems = (events.data ?? []).filter(isEffectivelyVisible);

  const fullCalendarEvents: EventInput[] = visibleItems.map((item) => {
    const colors = colorsForItem(item);
    return {
      id: item.id,
      title: item.title,
      start: item.when,
      display: 'block',
      backgroundColor: colors.background,
      borderColor: colors.border,
      textColor: colors.text ?? colors.border,
      classNames: item.personalEvent?.isCompleted ? ['event-completed'] : [],
      extendedProps: {
        eventId: item.eventId,
        isDraggable: item.personalEvent?.isDraggable ?? true,
        isCompleted: item.personalEvent?.isCompleted ?? false,
        description: item.personalEvent?.description ?? null,
      },
    };
  });

  function handleEventMove(eventId: string, newStart: Date, successMessage: string) {
    moveEventDate.mutate(
      { eventId, newStartAt: newStart.toISOString() },
      { onSuccess: () => showToast(successMessage), onError: () => showToast('Could not move that event.') }
    );
  }

  const prevMonthDate = new Date(viewStart.getFullYear(), viewStart.getMonth() - 1, 1);
  const nextMonthDate = new Date(viewStart.getFullYear(), viewStart.getMonth() + 1, 1);

  function moveToOtherMonth(target: Date, label: string) {
    return (e: ReactDragEvent) => {
      e.preventDefault();
      const eventId = e.dataTransfer.getData('text/plain');
      if (!eventId) return;
      const existing = calendarRef.current?.getApi().getEventById(eventId);
      const originalStart = existing?.start ?? new Date();
      const newStart = new Date(target.getFullYear(), target.getMonth(), originalStart.getDate(),
        originalStart.getHours(), originalStart.getMinutes(), originalStart.getSeconds());
      handleEventMove(eventId, newStart, `✓ Moved to ${label}`);
    };
  }

  return (
    <div className="mx-auto max-w-6xl px-6 py-10">
      <header className="mb-8 flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold text-gray-800">Brian</h1>
          {claims?.unique_name ? (
            <p className="mt-0.5 text-xs text-gray-500">Welcome, @{String(claims.unique_name)}</p>
          ) : null}
        </div>
        <div className="flex items-center gap-2">
          <PluginMenu onOpenEvent={(event) => setEditorState({ date: new Date(event.startAt), eventId: event.id })} />
          <button
            type="button"
            onClick={logout}
            className="rounded-md border border-gray-300 px-4 py-2 text-xs text-gray-500 transition hover:bg-gray-100"
          >
            Sign Out
          </button>
        </div>
      </header>

      <div className="flex gap-4">
        <FolderTree />

        <div className="min-w-0 flex-1">
          <div className="mb-3 flex items-center gap-2">
            <div
              onClick={() => calendarRef.current?.getApi().prev()}
              onDragOver={(e) => e.preventDefault()}
              onDrop={moveToOtherMonth(prevMonthDate, monthLabel(prevMonthDate))}
              className="flex-1 cursor-pointer rounded-md border-2 border-dashed border-orange-300 bg-orange-50 px-3 py-2 text-center text-xs text-orange-700 transition hover:bg-orange-100"
            >
              ← {monthLabel(prevMonthDate)}
            </div>
            <div className="px-3 text-sm font-semibold text-gray-800">{monthLabel(viewStart)}</div>
            <div
              onClick={() => calendarRef.current?.getApi().next()}
              onDragOver={(e) => e.preventDefault()}
              onDrop={moveToOtherMonth(nextMonthDate, monthLabel(nextMonthDate))}
              className="flex-1 cursor-pointer rounded-md border-2 border-dashed border-green-400 bg-green-50 px-3 py-2 text-center text-xs text-green-700 transition hover:bg-green-100"
            >
              {monthLabel(nextMonthDate)} →
            </div>
          </div>

          <AnimatePresence>
            {toast && (
              <motion.div
                initial={{ opacity: 0, y: -6 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0 }}
                className="mb-3 rounded-md bg-gray-800 px-3 py-2 text-center text-xs text-white"
              >
                {toast}
              </motion.div>
            )}
          </AnimatePresence>

          <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
            <FullCalendar
              ref={calendarRef}
              plugins={[dayGridPlugin, timeGridPlugin, listPlugin]}
              initialView="dayGridMonth"
              headerToolbar={{
                left: 'today',
                center: '',
                right: 'listMonth,dayGridMonth,timeGridWeek',
              }}
              events={fullCalendarEvents}
              height="auto"
              datesSet={(arg) => setViewStart(arg.view.currentStart)}
              eventContent={(arg) => {
                const description = arg.event.extendedProps.description as string | null | undefined;
                return (
                  <div className="overflow-hidden px-1 leading-tight">
                    <div className="truncate">{arg.event.title}</div>
                    {description && <div className="truncate text-[10px] opacity-80">{description}</div>}
                  </div>
                );
              }}
              dayCellContent={(arg) => {
                const dayMemos = memosByDate.get(toDateOnly(arg.date)) ?? [];
                return (
                  <div className="flex w-full flex-col items-start">
                    <span>{arg.dayNumberText}</span>
                    {dayMemos.length > 0 && (
                      <div
                        className="mt-0.5 w-full truncate text-left text-[10px] text-gray-500"
                        title={dayMemos.join(', ')}
                      >
                        📝 {dayMemos[0]}
                        {dayMemos.length > 1 ? ` +${dayMemos.length - 1}` : ''}
                      </div>
                    )}
                  </div>
                );
              }}
              eventDidMount={(arg) => {
                const isDraggable = arg.event.extendedProps.isDraggable !== false;
                const eventId = (arg.event.extendedProps.eventId as string | undefined) ?? arg.event.id;
                const el = arg.el as HTMLElement;
                el.draggable = isDraggable;
                el.style.cursor = isDraggable ? 'grab' : 'not-allowed';
                el.style.opacity = isDraggable ? '1' : '0.6';
                el.addEventListener('dragstart', (e) => {
                  if (!isDraggable) {
                    e.preventDefault();
                    return;
                  }
                  el.style.opacity = '0.5';
                  e.dataTransfer?.setData('text/plain', eventId);
                });
                el.addEventListener('dragend', () => {
                  el.style.opacity = isDraggable ? '1' : '0.6';
                });
                el.addEventListener('dblclick', (e) => {
                  e.stopPropagation();
                  const personalEvent = eventById.get(eventId);
                  if (personalEvent) setEditorState({ date: new Date(personalEvent.startAt), eventId: personalEvent.id });
                });
              }}
              dayCellDidMount={(arg) => {
                const cellDate = arg.date;
                arg.el.addEventListener('dragover', (e) => {
                  e.preventDefault();
                  arg.el.classList.add('ring-2', 'ring-blue-400');
                });
                arg.el.addEventListener('dragleave', () => {
                  arg.el.classList.remove('ring-2', 'ring-blue-400');
                });
                arg.el.addEventListener('drop', (e) => {
                  e.preventDefault();
                  arg.el.classList.remove('ring-2', 'ring-blue-400');
                  const dragEvent = e as DragEvent;
                  const eventId = dragEvent.dataTransfer?.getData('text/plain');
                  if (!eventId) return;

                  const existing = calendarRef.current?.getApi().getEventById(eventId);
                  const originalStart = existing?.start ?? cellDate;
                  const newStart = new Date(cellDate);
                  newStart.setHours(originalStart.getHours(), originalStart.getMinutes(), originalStart.getSeconds(), 0);

                  handleEventMove(
                    eventId,
                    newStart,
                    `Moved to ${cellDate.toLocaleDateString('en-US', { month: 'long', day: 'numeric' })}`
                  );
                });
                arg.el.addEventListener('dblclick', () => {
                  setEditorState({ date: cellDate });
                });
              }}
            />
          </div>

          <div className="mt-6 rounded-lg bg-white p-6 shadow-sm">
            <h2 className="mb-4 text-base font-semibold text-gray-800">Upcoming</h2>
            <EventList items={visibleItems} isLoading={events.isLoading} isError={events.isError} />
          </div>

          <ConnectionGroupbox config={PLATFORM_CONFIGS.holodex} />
          <ConnectionGroupbox config={PLATFORM_CONFIGS.youtube} />
        </div>
      </div>

      <AnimatePresence>
        {editorState && (
          <EventEditBox
            date={editorState.date}
            existingEvent={editorState.eventId ? eventById.get(editorState.eventId) : undefined}
            onClose={() => setEditorState(null)}
          />
        )}
      </AnimatePresence>

      {isCreator && (
        <div className="fixed bottom-4 right-4 flex gap-2">
          <button
            type="button"
            onClick={() => navigate('/vtuberhub')}
            className="rounded-md border border-gray-300 bg-white px-4 py-2.5 text-xs text-gray-600 shadow-sm transition hover:bg-gray-100"
          >
            Edit Public Website
          </button>
          <button
            type="button"
            onClick={() => navigate('/vtuberhub')}
            className="rounded-md bg-blue-500 px-4 py-2.5 text-xs text-white shadow-sm transition hover:bg-blue-600"
          >
            VTuberHub
          </button>
        </div>
      )}
    </div>
  );
}
