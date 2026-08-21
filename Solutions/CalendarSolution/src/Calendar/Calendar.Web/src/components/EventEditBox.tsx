import { useState } from 'react';
import { AnimatePresence, motion } from 'motion/react';
import type { CountdownCategory, PersonalEvent, RecurrenceType } from '../lib/queries/calendarQueries';
import { useCreateEvent, useCreateReminderPlan, useDeleteEvent, useSetEventAutoDefer, useSetEventCompletion, useSetEventCountdownCategory, useSetEventRecurrence, useUpdateEvent } from '../lib/queries/eventQueries';
import { useCreateMemo, useDeleteMemo, useMemos } from '../lib/queries/memoQueries';

const COUNTDOWN_STYLES: Record<CountdownCategory, string> = {
  Birthday: 'bg-pink-50 border-pink-300 text-pink-700',
  Holiday: 'bg-green-50 border-green-300 text-green-700',
  Anniversary: 'bg-purple-50 border-purple-300 text-purple-700',
  Exam: 'bg-red-50 border-red-300 text-red-700',
  Default: 'bg-gray-50 border-gray-300 text-gray-700',
};
const COUNTDOWN_CATEGORIES: CountdownCategory[] = ['Birthday', 'Holiday', 'Anniversary', 'Exam', 'Default'];

function toLocalInputValue(iso: string): string {
  const d = new Date(iso);
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

function fromLocalInputValue(value: string): string {
  return new Date(value).toISOString();
}

function toDateOnly(d: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

interface EventEditBoxProps {
  date: Date;
  existingEvent?: PersonalEvent;
  onClose: () => void;
}

export function EventEditBox({ date, existingEvent, onClose }: EventEditBoxProps) {
  const [tab, setTab] = useState<'event' | 'memo'>('event');

  const createEvent = useCreateEvent();
  const updateEvent = useUpdateEvent();
  const deleteEvent = useDeleteEvent();
  const setCompletion = useSetEventCompletion();
  const setCountdown = useSetEventCountdownCategory();
  const setRecurrence = useSetEventRecurrence();
  const setAutoDefer = useSetEventAutoDefer();
  const createReminderPlan = useCreateReminderPlan();

  const [title, setTitle] = useState(existingEvent?.title ?? '');
  const [description, setDescription] = useState(existingEvent?.description ?? '');
  const [location, setLocation] = useState(existingEvent?.location ?? '');
  const [isAllDay, setIsAllDay] = useState(existingEvent?.isAllDay ?? false);
  const [startAt, setStartAt] = useState(toLocalInputValue(existingEvent?.startAt ?? date.toISOString()));
  const [endAt, setEndAt] = useState(existingEvent?.endAt ? toLocalInputValue(existingEvent.endAt) : '');
  const [error, setError] = useState<string | null>(null);

  const [showRepeat, setShowRepeat] = useState(false);
  const [recurrenceType, setRecurrenceType] = useState<RecurrenceType>(existingEvent?.recurrenceType ?? 'None');
  const [recurrenceEndDate, setRecurrenceEndDate] = useState('');

  const [showReminder, setShowReminder] = useState(false);
  const [reminderRecurrence, setReminderRecurrence] = useState<RecurrenceType>('None');
  const [reminderEndDate, setReminderEndDate] = useState('');
  const [reminderCount, setReminderCount] = useState(1);
  const [reminderTimes, setReminderTimes] = useState<string[]>(['09:00']);
  const [reminderMethod, setReminderMethod] = useState<'Email' | 'Push' | 'InApp'>('InApp');
  const [reminderMessage, setReminderMessage] = useState<string | null>(null);

  const [showCountdown, setShowCountdown] = useState(false);

  const dateOnly = toDateOnly(date);
  const memos = useMemos(dateOnly, dateOnly);
  const createMemo = useCreateMemo();
  const deleteMemo = useDeleteMemo();
  const [memoDraft, setMemoDraft] = useState('');

  function handleSave() {
    setError(null);
    if (!title.trim()) {
      setError('Title is required.');
      return;
    }
    const input = {
      title: title.trim(),
      description: description.trim() || null,
      location: location.trim() || null,
      startAt: fromLocalInputValue(startAt),
      endAt: endAt ? fromLocalInputValue(endAt) : null,
      isAllDay,
      recurrenceRule: null,
    };

    if (existingEvent) {
      updateEvent.mutate({ eventId: existingEvent.id, ...input }, {
        onSuccess: onClose,
        onError: () => setError('Could not save changes.'),
      });
    } else {
      createEvent.mutate(input, {
        onSuccess: onClose,
        onError: () => setError('Could not create the event.'),
      });
    }
  }

  function handleDelete() {
    if (!existingEvent) return;
    if (!confirm(`Delete "${existingEvent.title}"?`)) return;
    deleteEvent.mutate(existingEvent.id, { onSuccess: onClose });
  }

  function handleSaveRepeat() {
    if (!existingEvent) return;
    setRecurrence.mutate({
      eventId: existingEvent.id,
      recurrenceType,
      recurrenceEndDate: recurrenceEndDate ? new Date(recurrenceEndDate).toISOString() : null,
    });
  }

  function handleSaveCountdown(category: CountdownCategory | null) {
    if (!existingEvent) return;
    setCountdown.mutate({ eventId: existingEvent.id, category });
  }

  function handleCreateReminderPlan() {
    if (!existingEvent) return;
    setReminderMessage(null);
    createReminderPlan.mutate(
      {
        eventId: existingEvent.id,
        firstTriggerAt: new Date(startAt).toISOString(),
        recurrenceType: reminderRecurrence,
        endDate: reminderEndDate ? new Date(reminderEndDate).toISOString() : null,
        maxOccurrences: reminderCount,
        timesOfDay: reminderTimes,
        method: reminderMethod,
      },
      {
        onSuccess: (ids) => setReminderMessage(`Created ${ids.length} reminder${ids.length === 1 ? '' : 's'}.`),
        onError: () => setReminderMessage('Could not create the reminder plan.'),
      }
    );
  }

  function handleAddMemo() {
    const text = memoDraft.trim();
    if (!text) return;
    createMemo.mutate({ date: dateOnly, text }, { onSuccess: () => setMemoDraft('') });
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30 px-4" onClick={onClose}>
      <motion.div
        initial={{ opacity: 0, scale: 0.97 }}
        animate={{ opacity: 1, scale: 1 }}
        exit={{ opacity: 0 }}
        transition={{ duration: 0.12 }}
        onClick={(e) => e.stopPropagation()}
        className="max-h-[85vh] w-full max-w-lg overflow-y-auto rounded-lg bg-white p-5 shadow-xl"
      >
        <div className="mb-4 flex items-center justify-between">
          <div className="flex gap-1 rounded-md bg-gray-100 p-1 text-xs">
            <button
              type="button"
              onClick={() => setTab('event')}
              className={`rounded px-3 py-1 ${tab === 'event' ? 'bg-white shadow-sm text-gray-800' : 'text-gray-500'}`}
            >
              Event
            </button>
            <button
              type="button"
              onClick={() => setTab('memo')}
              className={`rounded px-3 py-1 ${tab === 'memo' ? 'bg-white shadow-sm text-gray-800' : 'text-gray-500'}`}
            >
              Memo
            </button>
          </div>
          <button type="button" onClick={onClose} className="text-gray-400 hover:text-gray-700" aria-label="Close">
            ✕
          </button>
        </div>

        {tab === 'event' && (
          <div>
            <input
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Title"
              className="mb-2 w-full rounded-md border border-gray-300 px-3 py-2 text-sm font-medium"
            />
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Description"
              rows={2}
              className="mb-2 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
            />
            <input
              type="text"
              value={location}
              onChange={(e) => setLocation(e.target.value)}
              placeholder="Location"
              className="mb-2 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
            />

            <label className="mb-2 flex items-center gap-2 text-xs text-gray-600">
              <input type="checkbox" checked={isAllDay} onChange={(e) => setIsAllDay(e.target.checked)} />
              All day
            </label>

            <div className="mb-3 flex gap-2">
              <label className="flex-1 text-xs text-gray-500">
                Start
                <input
                  type={isAllDay ? 'date' : 'datetime-local'}
                  value={isAllDay ? startAt.slice(0, 10) : startAt}
                  onChange={(e) => setStartAt(isAllDay ? `${e.target.value}T00:00` : e.target.value)}
                  className="mt-0.5 w-full rounded-md border border-gray-300 px-2 py-1.5 text-sm"
                />
              </label>
              <label className="flex-1 text-xs text-gray-500">
                End (optional)
                <input
                  type={isAllDay ? 'date' : 'datetime-local'}
                  value={isAllDay ? endAt.slice(0, 10) : endAt}
                  onChange={(e) => setEndAt(isAllDay ? `${e.target.value}T00:00` : e.target.value)}
                  className="mt-0.5 w-full rounded-md border border-gray-300 px-2 py-1.5 text-sm"
                />
              </label>
            </div>

            {error && <div className="mb-3 text-xs text-red-600">{error}</div>}

            <div className="mb-3 flex flex-wrap gap-2">
              <button type="button" onClick={() => setShowRepeat((v) => !v)} className="rounded-md border border-gray-300 px-3 py-1 text-xs text-gray-600 hover:bg-gray-50">
                🔁 Repeat
              </button>
              {existingEvent && (
                <button type="button" onClick={() => setShowReminder((v) => !v)} className="rounded-md border border-gray-300 px-3 py-1 text-xs text-gray-600 hover:bg-gray-50">
                  ⏰ Reminder
                </button>
              )}
              <button type="button" onClick={() => setShowCountdown((v) => !v)} className="rounded-md border border-gray-300 px-3 py-1 text-xs text-gray-600 hover:bg-gray-50">
                🎉 Countdown
              </button>
            </div>

            <AnimatePresence>
              {showRepeat && (
                <motion.div initial={{ height: 0, opacity: 0 }} animate={{ height: 'auto', opacity: 1 }} exit={{ height: 0, opacity: 0 }} className="mb-3 overflow-hidden rounded-md border border-gray-200 bg-gray-50 p-3 text-xs">
                  <div className="mb-2 flex items-center gap-2">
                    <select value={recurrenceType} onChange={(e) => setRecurrenceType(e.target.value as RecurrenceType)} className="rounded border border-gray-300 px-2 py-1">
                      <option value="None">Does not repeat</option>
                      <option value="Daily">Daily</option>
                      <option value="Weekly">Weekly (e.g. class schedule)</option>
                      <option value="Monthly">Monthly</option>
                    </select>
                    {recurrenceType !== 'None' && (
                      <input type="date" value={recurrenceEndDate} onChange={(e) => setRecurrenceEndDate(e.target.value)} className="rounded border border-gray-300 px-2 py-1" />
                    )}
                  </div>
                  <p className="mb-2 text-gray-400">Saved for later - the calendar doesn't show repeated occurrences yet.</p>
                  {existingEvent ? (
                    <button type="button" onClick={handleSaveRepeat} className="rounded bg-blue-500 px-3 py-1 text-white">Save repeat rule</button>
                  ) : (
                    <p className="text-gray-400">Save the event first to set its repeat rule.</p>
                  )}
                </motion.div>
              )}
            </AnimatePresence>

            <AnimatePresence>
              {showReminder && existingEvent && (
                <motion.div initial={{ height: 0, opacity: 0 }} animate={{ height: 'auto', opacity: 1 }} exit={{ height: 0, opacity: 0 }} className="mb-3 overflow-hidden rounded-md border border-gray-200 bg-gray-50 p-3 text-xs">
                  <div className="mb-2 flex flex-wrap items-center gap-2">
                    <select value={reminderRecurrence} onChange={(e) => setReminderRecurrence(e.target.value as RecurrenceType)} className="rounded border border-gray-300 px-2 py-1">
                      <option value="None">Once</option>
                      <option value="Daily">Daily</option>
                      <option value="Weekly">Weekly</option>
                      <option value="Monthly">Monthly</option>
                    </select>
                    <label>
                      Count
                      <input type="number" min={1} max={50} value={reminderCount} onChange={(e) => setReminderCount(Number(e.target.value))} className="ml-1 w-14 rounded border border-gray-300 px-1 py-1" />
                    </label>
                    <input type="date" value={reminderEndDate} onChange={(e) => setReminderEndDate(e.target.value)} placeholder="End date" className="rounded border border-gray-300 px-2 py-1" />
                    <select value={reminderMethod} onChange={(e) => setReminderMethod(e.target.value as 'Email' | 'Push' | 'InApp')} className="rounded border border-gray-300 px-2 py-1">
                      <option value="InApp">In-app</option>
                      <option value="Email">Email</option>
                      <option value="Push">Push</option>
                    </select>
                  </div>
                  <div className="mb-2 flex flex-wrap items-center gap-1">
                    {reminderTimes.map((t, i) => (
                      <input
                        key={i}
                        type="time"
                        value={t}
                        onChange={(e) => setReminderTimes((prev) => prev.map((p, idx) => (idx === i ? e.target.value : p)))}
                        className="rounded border border-gray-300 px-1 py-1"
                      />
                    ))}
                    <button type="button" onClick={() => setReminderTimes((prev) => [...prev, '09:00'])} className="text-blue-500">+ time</button>
                  </div>
                  {reminderMessage && <p className="mb-2 text-gray-500">{reminderMessage}</p>}
                  <button type="button" onClick={handleCreateReminderPlan} className="rounded bg-blue-500 px-3 py-1 text-white">Create reminders</button>
                </motion.div>
              )}
            </AnimatePresence>

            <AnimatePresence>
              {showCountdown && (
                <motion.div initial={{ height: 0, opacity: 0 }} animate={{ height: 'auto', opacity: 1 }} exit={{ height: 0, opacity: 0 }} className="mb-3 overflow-hidden">
                  <div className="flex flex-wrap gap-2">
                    <button
                      type="button"
                      onClick={() => existingEvent && handleSaveCountdown(null)}
                      className={`rounded-md border px-2 py-1 text-xs ${!existingEvent?.countdownCategory ? 'border-gray-500 bg-gray-100' : 'border-gray-200 text-gray-400'}`}
                    >
                      None
                    </button>
                    {COUNTDOWN_CATEGORIES.map((category) => (
                      <button
                        key={category}
                        type="button"
                        onClick={() => existingEvent && handleSaveCountdown(category)}
                        disabled={!existingEvent}
                        className={`rounded-md border px-2 py-1 text-xs ${COUNTDOWN_STYLES[category]} ${existingEvent?.countdownCategory === category ? 'ring-2 ring-offset-1' : ''}`}
                      >
                        {category}
                      </button>
                    ))}
                  </div>
                  {!existingEvent && <p className="mt-1 text-xs text-gray-400">Save the event first to set a countdown category.</p>}
                </motion.div>
              )}
            </AnimatePresence>

            {existingEvent && (
              <div className="mb-3 flex items-center justify-between rounded-md border border-gray-200 px-3 py-2 text-xs">
                <label className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    checked={existingEvent.isCompleted}
                    onChange={(e) => setCompletion.mutate({ eventId: existingEvent.id, isCompleted: e.target.checked })}
                  />
                  <span className={existingEvent.isCompleted ? 'text-gray-400 line-through' : ''}>Finished</span>
                </label>
                {existingEvent.isAllDay && (
                  <label className="flex items-center gap-2">
                    <input
                      type="checkbox"
                      checked={existingEvent.autoDeferEnabled}
                      onChange={(e) => setAutoDefer.mutate({ eventId: existingEvent.id, autoDeferEnabled: e.target.checked })}
                    />
                    Auto-defer if incomplete
                  </label>
                )}
              </div>
            )}

            <div className="flex items-center justify-between">
              {existingEvent ? (
                <button type="button" onClick={handleDelete} className="text-xs text-red-500 hover:text-red-700">
                  Delete
                </button>
              ) : (
                <span />
              )}
              <button
                type="button"
                onClick={handleSave}
                disabled={createEvent.isPending || updateEvent.isPending}
                className="rounded-md bg-blue-500 px-5 py-2 text-sm text-white hover:bg-blue-600 disabled:opacity-50"
              >
                Save
              </button>
            </div>
          </div>
        )}

        {tab === 'memo' && (
          <div>
            <p className="mb-2 text-xs text-gray-500">Quick notes for {dateOnly}</p>
            <div className="mb-3 max-h-48 overflow-y-auto rounded-md border border-gray-100">
              {memos.isLoading && <p className="p-3 text-xs text-gray-400">Loading...</p>}
              {!memos.isLoading && (memos.data ?? []).length === 0 && (
                <p className="p-3 text-xs text-gray-400">No memos yet for this date.</p>
              )}
              {(memos.data ?? []).map((memo) => (
                <div key={memo.id} className="flex items-center justify-between border-b border-gray-100 px-3 py-2 text-sm last:border-none">
                  <span>{memo.text}</span>
                  <button type="button" onClick={() => deleteMemo.mutate(memo.id)} className="text-xs text-gray-300 hover:text-red-500">✕</button>
                </div>
              ))}
            </div>
            <input
              autoFocus
              type="text"
              value={memoDraft}
              onChange={(e) => setMemoDraft(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  e.preventDefault();
                  handleAddMemo();
                }
              }}
              placeholder="Type a quick note and press Enter..."
              className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
            />
          </div>
        )}
      </motion.div>
    </div>
  );
}
