import { AnimatePresence, motion } from 'motion/react';
import type { CalendarItem } from '../lib/queries/calendarQueries';

const BADGE_STYLES: Record<CalendarItem['kind'], string> = {
  personal: 'bg-blue-50 text-blue-600',
  live: 'bg-red-50 text-red-600',
  upcoming: 'bg-amber-50 text-amber-700',
};

function badgeLabel(item: CalendarItem): string {
  if (item.kind === 'personal') return 'Personal';
  if (item.kind === 'live') return 'Live';
  return item.platform ?? 'Upcoming';
}

function formatDateTime(iso: string): string {
  return new Date(iso).toLocaleString(undefined, {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
  });
}

interface EventListProps {
  items: CalendarItem[];
  isLoading: boolean;
  isError: boolean;
}

export function EventList({ items, isLoading, isError }: EventListProps) {
  if (isLoading) {
    return <p className="py-8 text-center text-sm text-gray-400">Loading your calendar...</p>;
  }

  if (isError) {
    return <p className="py-8 text-center text-sm text-gray-400">Failed to load your calendar.</p>;
  }

  if (items.length === 0) {
    return <p className="py-8 text-center text-sm text-gray-400">Nothing on your calendar in the next 30 days.</p>;
  }

  return (
    <div>
      <AnimatePresence initial={false}>
        {items.map((item) => (
          <motion.div
            key={item.id}
            layout
            initial={{ opacity: 0, y: -8 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.2 }}
            className="flex items-center justify-between border-b border-gray-100 py-3 last:border-none"
          >
            <div>
              <div className="text-sm font-medium text-gray-800">{item.title}</div>
              <div className="mt-0.5 text-xs text-gray-400">{formatDateTime(item.when)}</div>
            </div>
            <span className={`rounded-full px-2 py-0.5 text-[11px] font-semibold uppercase ${BADGE_STYLES[item.kind]}`}>
              {badgeLabel(item)}
            </span>
          </motion.div>
        ))}
      </AnimatePresence>
    </div>
  );
}
