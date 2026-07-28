import { useState } from 'react';
import { AnimatePresence, motion } from 'motion/react';
import { Link, useNavigate } from 'react-router-dom';
import type { PlatformConfig } from '../config/platforms';
import {
  useConnectionStatus,
  useFollowedChannels,
  useSaveCredential,
} from '../lib/queries/platformQueries';

export function ConnectionGroupbox({ config }: { config: PlatformConfig }) {
  const [showForm, setShowForm] = useState(false);
  const [apiKey, setApiKey] = useState('');
  const navigate = useNavigate();

  const status = useConnectionStatus(config);
  const isConnected = status.data?.hasCredential ?? false;
  const followed = useFollowedChannels(config, isConnected);
  const saveCredential = useSaveCredential(config);

  const label = followed.data?.length === 1 ? 'VTuber' : 'VTubers';

  return (
    <fieldset className="mt-4 rounded-lg border border-gray-300 bg-white p-5">
      <legend className="px-2 text-sm font-semibold text-gray-800">{config.label}</legend>

      {config.groupboxHint && <p className="mb-2.5 text-xs text-gray-400">{config.groupboxHint}</p>}

      <div className="flex items-center justify-between py-2">
        <span className="text-sm font-medium text-gray-800">{config.label}</span>
        {isConnected ? (
          <span className="rounded-full bg-green-50 px-2.5 py-1 text-[11px] font-semibold uppercase text-green-700">
            Connected
          </span>
        ) : (
          <button
            type="button"
            onClick={() => setShowForm(true)}
            className="rounded-md bg-blue-500 px-4 py-2 text-xs text-white transition hover:bg-blue-600"
          >
            Connect {config.label}
          </button>
        )}
      </div>

      <AnimatePresence initial={false}>
        {showForm && !isConnected && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: 'auto', opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            transition={{ duration: 0.2 }}
            className="overflow-hidden"
          >
            <div className="mt-2 flex gap-2">
              <input
                type="text"
                value={apiKey}
                onChange={(e) => setApiKey(e.target.value)}
                placeholder={`${config.label} API key`}
                className="flex-1 rounded-md border border-gray-300 px-3 py-2 text-sm"
              />
              <button
                type="button"
                onClick={() =>
                  saveCredential.mutate(apiKey.trim(), {
                    onSuccess: () => navigate(`/follow/${config.key}`),
                  })
                }
                disabled={!apiKey.trim() || saveCredential.isPending}
                className="rounded-md bg-blue-500 px-4 py-2 text-xs text-white transition hover:bg-blue-600 disabled:opacity-50"
              >
                Save
              </button>
              <button
                type="button"
                onClick={() => {
                  setShowForm(false);
                  setApiKey('');
                  saveCredential.reset();
                }}
                className="rounded-md border border-gray-300 px-3.5 py-2 text-xs text-gray-500"
              >
                Cancel
              </button>
            </div>
            {saveCredential.isError && (
              <p className="mt-1.5 text-xs text-red-600">
                Could not save your {config.label} API key. Please check it and try again.
              </p>
            )}
          </motion.div>
        )}
      </AnimatePresence>

      {isConnected && (
        <div>
          <div className="flex items-center justify-between py-2">
            <span className="text-sm font-medium text-gray-800">
              Following {followed.data?.length ?? 0} {label}
            </span>
            <Link to={`/follow/${config.key}`} className="text-xs text-blue-500 hover:underline">
              Manage VTubers
            </Link>
          </div>

          {followed.data && followed.data.length === 0 && (
            <p className="mt-2 text-xs text-gray-400">You're not following any VTubers yet.</p>
          )}

          <div className="mt-2">
            {followed.data?.map((c) => (
              <div key={c.channelId} className="flex items-center gap-2.5 border-b border-gray-100 py-2 last:border-none">
                {c.photoUrl && <img src={c.photoUrl} alt="" className="h-6 w-6 rounded-full object-cover" />}
                <span className="text-sm text-gray-800">{c.englishName || c.name}</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </fieldset>
  );
}
