import { useState } from 'react';
import { AnimatePresence, motion } from 'motion/react';
import { useCreateNote, useDeleteNote, useNotes, useUpdateNote } from '../lib/queries/noteQueries';
import { useSearchEvents } from '../lib/queries/eventQueries';
import type { PersonalEvent } from '../lib/queries/calendarQueries';

interface PluginMenuProps {
  onOpenEvent: (event: PersonalEvent) => void;
}

export function PluginMenu({ onOpenEvent }: PluginMenuProps) {
  const [open, setOpen] = useState(false);
  const [section, setSection] = useState<'notes' | 'search'>('notes');

  const notes = useNotes();
  const createNote = useCreateNote();
  const updateNote = useUpdateNote();
  const deleteNote = useDeleteNote();
  const [editingNoteId, setEditingNoteId] = useState<string | null>(null);
  const [noteTitle, setNoteTitle] = useState('');
  const [noteContent, setNoteContent] = useState('');

  const [searchInput, setSearchInput] = useState('');
  const search = useSearchEvents(searchInput);

  function startNewNote() {
    setEditingNoteId('new');
    setNoteTitle('');
    setNoteContent('');
  }

  function startEditNote(note: { id: string; title: string; content: string }) {
    setEditingNoteId(note.id);
    setNoteTitle(note.title);
    setNoteContent(note.content);
  }

  function saveNote() {
    if (!noteTitle.trim()) return;
    if (editingNoteId === 'new') {
      createNote.mutate({ title: noteTitle.trim(), content: noteContent }, { onSuccess: () => setEditingNoteId(null) });
    } else if (editingNoteId) {
      updateNote.mutate({ noteId: editingNoteId, title: noteTitle.trim(), content: noteContent }, { onSuccess: () => setEditingNoteId(null) });
    }
  }

  return (
    <>
      <button
        type="button"
        className="rounded border border-gray-300 bg-white px-3 py-1 text-xs text-gray-700 hover:bg-gray-50"
        onClick={() => setOpen((v) => !v)}
      >
        ⋯ Notes / Search
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
              <div className="flex gap-1 rounded-md bg-gray-100 p-1 text-xs">
                <button type="button" onClick={() => setSection('notes')} className={`rounded px-3 py-1 ${section === 'notes' ? 'bg-white shadow-sm' : 'text-gray-500'}`}>
                  Notes
                </button>
                <button type="button" onClick={() => setSection('search')} className={`rounded px-3 py-1 ${section === 'search' ? 'bg-white shadow-sm' : 'text-gray-500'}`}>
                  Search
                </button>
              </div>
              <button type="button" className="text-gray-400 hover:text-gray-700" onClick={() => setOpen(false)} aria-label="Close">
                ✕
              </button>
            </div>

            <div className="flex-1 overflow-y-auto p-4">
              {section === 'notes' && (
                <div>
                  {editingNoteId ? (
                    <div className="mb-4">
                      <input
                        type="text"
                        value={noteTitle}
                        onChange={(e) => setNoteTitle(e.target.value)}
                        placeholder="Title"
                        className="mb-2 w-full rounded-md border border-gray-300 px-3 py-2 text-sm font-medium"
                      />
                      <textarea
                        value={noteContent}
                        onChange={(e) => setNoteContent(e.target.value)}
                        rows={6}
                        placeholder="Write your note..."
                        className="mb-2 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                      />
                      <div className="flex justify-end gap-2">
                        <button type="button" onClick={() => setEditingNoteId(null)} className="text-xs text-gray-500">Cancel</button>
                        <button type="button" onClick={saveNote} className="rounded-md bg-blue-500 px-4 py-1.5 text-xs text-white hover:bg-blue-600">Save</button>
                      </div>
                    </div>
                  ) : (
                    <button type="button" onClick={startNewNote} className="mb-3 w-full rounded-md border border-dashed border-gray-300 py-2 text-xs text-gray-400 hover:text-gray-600">
                      + New Note
                    </button>
                  )}

                  {notes.isLoading && <p className="text-xs text-gray-400">Loading notes...</p>}
                  {(notes.data ?? []).map((note) => (
                    <div key={note.id} className="mb-2 rounded-md border border-gray-200 p-3">
                      <div className="mb-1 flex items-center justify-between">
                        <span className="text-sm font-medium text-gray-800">{note.title}</span>
                        <div className="flex gap-2 text-xs text-gray-400">
                          <button type="button" onClick={() => startEditNote(note)} className="hover:text-gray-700">Edit</button>
                          <button type="button" onClick={() => deleteNote.mutate(note.id)} className="hover:text-red-500">Delete</button>
                        </div>
                      </div>
                      <p className="whitespace-pre-wrap text-xs text-gray-500">{note.content}</p>
                    </div>
                  ))}
                </div>
              )}

              {section === 'search' && (
                <div>
                  <input
                    autoFocus
                    type="text"
                    value={searchInput}
                    onChange={(e) => setSearchInput(e.target.value)}
                    placeholder="Search your schedule..."
                    className="mb-3 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {search.isLoading && <p className="text-xs text-gray-400">Searching...</p>}
                  {(search.data ?? []).map((event) => (
                    <div
                      key={event.id}
                      onDoubleClick={() => {
                        onOpenEvent(event);
                        setOpen(false);
                      }}
                      className="cursor-pointer rounded-md border-b border-gray-100 px-2 py-2 text-sm hover:bg-gray-50"
                      title="Double-click to open"
                    >
                      <div className="font-medium text-gray-800">{event.title}</div>
                      <div className="text-xs text-gray-400">{new Date(event.startAt).toLocaleString()}</div>
                    </div>
                  ))}
                  {search.data && search.data.length === 0 && (
                    <p className="text-xs text-gray-400">No matches.</p>
                  )}
                </div>
              )}
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </>
  );
}
