import { useState } from 'react';
import { AnimatePresence, motion } from 'motion/react';
import { useChatAccount } from '../lib/queries/chatQueries';
import { STOAT_WEB_URL } from '../config/platforms';

export function ChatPanel() {
  const [open, setOpen] = useState(false);
  const [showCredentials, setShowCredentials] = useState(true);
  const { data: account, isLoading, isError } = useChatAccount(open);

  return (
    <>
      <button
        type="button"
        className="rounded border border-gray-300 bg-white px-3 py-1 text-xs text-gray-700 hover:bg-gray-50"
        onClick={() => setOpen((v) => !v)}
      >
        💬 Chat
      </button>

      <AnimatePresence>
        {open && (
          <motion.div
            className="fixed right-0 top-0 z-50 flex h-full w-full max-w-md flex-col border-l border-gray-300 bg-white shadow-xl"
            initial={{ x: '100%' }}
            animate={{ x: 0 }}
            exit={{ x: '100%' }}
            transition={{ duration: 0.2 }}
          >
            <div className="flex items-center justify-between border-b border-gray-200 px-4 py-3">
              <h2 className="text-sm font-medium text-gray-800">Chat</h2>
              <button
                type="button"
                className="text-gray-400 hover:text-gray-700"
                onClick={() => setOpen(false)}
                aria-label="Close chat"
              >
                ✕
              </button>
            </div>

            {isLoading && (
              <div className="p-4 text-sm text-gray-500">Setting up your chat account...</div>
            )}

            {isError && (
              <div className="p-4 text-sm text-red-600">
                Could not set up chat right now. Please try again later.
              </div>
            )}

            {account && showCredentials && (
              <div className="border-b border-amber-200 bg-amber-50 px-4 py-3 text-xs text-amber-900">
                <p className="mb-1 font-medium">
                  First time here? Log in below with (chat handle: {account.username}):
                </p>
                <p>
                  Email: <span className="font-mono">{account.email}</span>
                </p>
                <p>
                  Password: <span className="font-mono">{account.password}</span>
                </p>
                <button
                  type="button"
                  className="mt-2 text-amber-700 underline hover:text-amber-900"
                  onClick={() => setShowCredentials(false)}
                >
                  Hide
                </button>
              </div>
            )}

            {account && (
              <iframe
                src={STOAT_WEB_URL}
                title="Chat"
                className="w-full flex-1 border-0"
              />
            )}
          </motion.div>
        )}
      </AnimatePresence>
    </>
  );
}
