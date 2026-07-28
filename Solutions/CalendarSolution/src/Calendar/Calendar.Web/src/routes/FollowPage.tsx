import { useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { PLATFORM_CONFIGS } from '../config/platforms';
import {
  useBrowseChannels,
  useFollowedChannels,
  useSaveFollowed,
  useSearchChannels,
  type Channel,
} from '../lib/queries/platformQueries';

interface SelectableChannel extends Channel {
  selected: boolean;
}

export function FollowPage() {
  const { platform } = useParams<{ platform: 'holodex' | 'youtube' }>();
  const config = PLATFORM_CONFIGS[platform ?? 'holodex'];
  const navigate = useNavigate();

  // Keyed by channelId so selections survive across multiple searches.
  const [channelsById, setChannelsById] = useState<Map<string, SelectableChannel>>(new Map());
  const [searchInput, setSearchInput] = useState('');
  const [committedSearch, setCommittedSearch] = useState('');
  const [error, setError] = useState<string | null>(null);

  const followed = useFollowedChannels(config, true);
  const browse = useBrowseChannels(config);
  const search = useSearchChannels(config, committedSearch);
  const saveFollowed = useSaveFollowed(config);

  const upsert = (channels: Channel[], markSelected: boolean) => {
    setChannelsById((prev) => {
      const next = new Map(prev);
      for (const c of channels) {
        const existing = next.get(c.channelId);
        next.set(c.channelId, {
          ...c,
          selected: existing ? existing.selected : markSelected || !!c.isFollowed,
        });
      }
      return next;
    });
  };

  const loadedFollowed = useRef(false);
  useEffect(() => {
    if (followed.data && !loadedFollowed.current) {
      loadedFollowed.current = true;
      upsert(followed.data, true);
    }
  }, [followed.data]);

  useEffect(() => {
    if (browse.data) upsert(browse.data, false);
    if (browse.isError && config.browseFailureMessage) setError(config.browseFailureMessage);
  }, [browse.data, browse.isError]);

  useEffect(() => {
    if (search.data) upsert(search.data, false);
    if (search.isError) setError(config.searchErrorMessage);
  }, [search.data, search.isError]);

  const channels = useMemo(() => Array.from(channelsById.values()), [channelsById]);
  const selectedCount = channels.filter((c) => c.selected).length;

  function toggleChannel(channelId: string, selected: boolean) {
    setChannelsById((prev) => {
      const next = new Map(prev);
      const entry = next.get(channelId);
      if (entry) next.set(channelId, { ...entry, selected });
      return next;
    });
  }

  function toggleSelectAll() {
    const allSelected = channels.every((c) => c.selected);
    setChannelsById((prev) => {
      const next = new Map(prev);
      for (const [id, entry] of next) next.set(id, { ...entry, selected: !allSelected });
      return next;
    });
  }

  function handleSearch() {
    setError(null);
    if (searchInput.trim().length >= 2) setCommittedSearch(searchInput.trim());
  }

  function handleFinish() {
    setError(null);
    const selected = channels.filter((c) => c.selected);
    saveFollowed.mutate(selected, {
      onSuccess: () => navigate('/'),
      onError: () => setError('Failed to save your selected VTubers. Please try again.'),
    });
  }

  const isLoading = followed.isLoading || browse.isLoading;
  const emptyMessage =
    committedSearch.length > 0 ? 'No VTubers found for that search.' : 'No VTubers found. Try searching by name above.';

  return (
    <div className="mx-auto max-w-3xl px-6 py-10">
      <header className="mb-8">
        <h1 className="text-xl font-semibold text-gray-800">Brian</h1>
        <p className="mt-0.5 text-xs text-gray-500">Choose the {config.label} VTubers you want to follow</p>
      </header>

      <div className="rounded-lg bg-white p-6 shadow-sm">
        <Link to="/" className="mb-4 inline-block text-xs text-blue-500 hover:underline">
          &larr; Back to Calendar
        </Link>

        {config.followPageHint && <p className="mb-4 text-xs text-gray-400">{config.followPageHint}</p>}

        <div className="mb-4 flex gap-2">
          <input
            type="text"
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                e.preventDefault();
                handleSearch();
              }
            }}
            placeholder="Search Holodex for a VTuber..."
            className="flex-1 rounded-md border border-gray-300 px-3 py-2.5 text-sm"
          />
          <button
            type="button"
            onClick={handleSearch}
            className="rounded-md bg-blue-500 px-4 py-2.5 text-xs text-white transition hover:bg-blue-600"
          >
            Search
          </button>
        </div>

        {error && <div className="mb-3 text-xs text-red-600">{error}</div>}

        <div className="mb-2.5 flex items-center justify-between">
          <span className="text-xs text-gray-500">Select the VTubers to follow</span>
          <button type="button" onClick={toggleSelectAll} className="text-xs text-blue-500">
            Select all / none
          </button>
        </div>

        <div className="mb-4 max-h-90 overflow-y-auto rounded-md border border-gray-100">
          {isLoading && <p className="py-5 text-center text-sm text-gray-400">Loading VTubers from Holodex...</p>}
          {!isLoading && channels.length === 0 && (
            <p className="py-5 text-center text-sm text-gray-400">{emptyMessage}</p>
          )}
          {channels.map((c) => (
            <label
              key={c.channelId}
              className="flex cursor-pointer items-center gap-2.5 border-b border-gray-100 px-3 py-2.5 text-sm text-gray-800 last:border-none"
            >
              <input
                type="checkbox"
                checked={c.selected}
                onChange={(e) => toggleChannel(c.channelId, e.target.checked)}
              />
              {c.photoUrl && <img src={c.photoUrl} alt="" className="h-7 w-7 rounded-full object-cover" />}
              <span>{c.englishName || c.name}</span>
            </label>
          ))}
        </div>

        <div className="flex items-center justify-between">
          <span className="text-xs text-gray-500">
            {selectedCount === 0 ? 'No VTubers selected' : `${selectedCount} VTuber${selectedCount === 1 ? '' : 's'} selected`}
          </span>
          <button
            type="button"
            onClick={handleFinish}
            disabled={saveFollowed.isPending}
            className="rounded-md bg-blue-500 px-5 py-2.5 text-sm text-white transition hover:bg-blue-600 disabled:opacity-50"
          >
            Finished
          </button>
        </div>
      </div>
    </div>
  );
}
