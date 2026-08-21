import { useState } from 'react';
import { AnimatePresence, motion } from 'motion/react';
import {
  useCalendarSettings,
  useCreateFolder,
  useCreateSubfolder,
  useDeleteFolder,
  useDeleteSubfolder,
  useFolderTree,
  useMoveEventToFolder,
  useRenameFolder,
  useRenameSubfolder,
  useSetFolderLock,
  useSetFolderVisibility,
  useSetSubfolderLock,
  useSetSubfolderVisibility,
  useUpdateCalendarSettings,
  type Folder,
  type Subfolder,
} from '../lib/queries/folderQueries';

function LockIcon({ locked, disabled }: { locked: boolean; disabled?: boolean }) {
  return (
    <span className={disabled ? 'opacity-40' : ''} title={locked ? 'Locked' : 'Unlocked'}>
      {locked ? '🔒' : '🔓'}
    </span>
  );
}

function EventChip({ event }: { event: { id: string; title: string; isDraggable: boolean } }) {
  return (
    <div
      draggable={event.isDraggable}
      onDragStart={(e) => {
        if (!event.isDraggable) {
          e.preventDefault();
          return;
        }
        e.dataTransfer.setData('text/plain', event.id);
      }}
      className={`ml-6 truncate rounded px-2 py-1 text-xs text-gray-600 ${
        event.isDraggable ? 'cursor-grab hover:bg-gray-100' : 'cursor-not-allowed opacity-60'
      }`}
      title={event.title}
    >
      {event.title}
    </div>
  );
}

function SubfolderRow({ folder, subfolder }: { folder: Folder; subfolder: Subfolder }) {
  const setVisibility = useSetSubfolderVisibility();
  const setLock = useSetSubfolderLock();
  const deleteSubfolder = useDeleteSubfolder();
  const moveEventToFolder = useMoveEventToFolder();
  const renameSubfolder = useRenameSubfolder();
  const [dragOver, setDragOver] = useState(false);
  const [renaming, setRenaming] = useState(false);
  const [nameDraft, setNameDraft] = useState(subfolder.name);
  const lockDisabled = folder.isLocked;

  function commitRename() {
    const name = nameDraft.trim();
    setRenaming(false);
    if (!name || name === subfolder.name) {
      setNameDraft(subfolder.name);
      return;
    }
    renameSubfolder.mutate({ subfolderId: subfolder.id, name });
  }

  return (
    <div
      onDragOver={(e) => {
        e.preventDefault();
        setDragOver(true);
      }}
      onDragLeave={() => setDragOver(false)}
      onDrop={(e) => {
        e.preventDefault();
        setDragOver(false);
        const eventId = e.dataTransfer.getData('text/plain');
        if (eventId) moveEventToFolder.mutate({ eventId, subfolderId: subfolder.id });
      }}
      className={`ml-4 rounded-md ${dragOver ? 'bg-blue-50 ring-1 ring-blue-300' : ''}`}
    >
      <div className="flex items-center gap-2 px-2 py-1 text-sm text-gray-700">
        <input
          type="checkbox"
          checked={subfolder.isVisible}
          onChange={(e) => setVisibility.mutate({ subfolderId: subfolder.id, isVisible: e.target.checked })}
        />
        {renaming ? (
          <input
            autoFocus
            type="text"
            value={nameDraft}
            onChange={(e) => setNameDraft(e.target.value)}
            onBlur={commitRename}
            onKeyDown={(e) => {
              if (e.key === 'Enter') commitRename();
              if (e.key === 'Escape') { setNameDraft(subfolder.name); setRenaming(false); }
            }}
            className="flex-1 rounded border border-gray-300 px-1 py-0.5 text-sm"
          />
        ) : (
          <span className="flex-1 truncate" onDoubleClick={() => setRenaming(true)}>
            {subfolder.name}
          </span>
        )}
        <button
          type="button"
          disabled={lockDisabled}
          onClick={() => setLock.mutate({ subfolderId: subfolder.id, isLocked: !subfolder.isLocked })}
          className="text-xs"
        >
          <LockIcon locked={subfolder.isLocked} disabled={lockDisabled} />
        </button>
        <button
          type="button"
          onClick={() => {
            if (confirm(`Delete subfolder "${subfolder.name}"? Its events won't be deleted, just un-filed.`)) {
              deleteSubfolder.mutate(subfolder.id);
            }
          }}
          className="text-xs text-gray-300 hover:text-red-500"
          aria-label={`Delete ${subfolder.name}`}
        >
          ✕
        </button>
      </div>

      {subfolder.events.map((event) => (
        <EventChip key={event.id} event={event} />
      ))}
    </div>
  );
}

function FolderRow({ folder }: { folder: Folder }) {
  const [expanded, setExpanded] = useState(true);
  const [addingSubfolder, setAddingSubfolder] = useState(false);
  const [newSubfolderName, setNewSubfolderName] = useState('');
  const setVisibility = useSetFolderVisibility();
  const setLock = useSetFolderLock();
  const deleteFolder = useDeleteFolder();
  const createSubfolder = useCreateSubfolder();
  const moveEventToFolder = useMoveEventToFolder();
  const renameFolder = useRenameFolder();
  const [dragOver, setDragOver] = useState(false);
  const [renaming, setRenaming] = useState(false);
  const [nameDraft, setNameDraft] = useState(folder.name);

  function commitRename() {
    const name = nameDraft.trim();
    setRenaming(false);
    if (!name || name === folder.name) {
      setNameDraft(folder.name);
      return;
    }
    renameFolder.mutate({ folderId: folder.id, name });
  }

  function handleCreateSubfolder() {
    const name = newSubfolderName.trim();
    if (!name) return;
    createSubfolder.mutate(
      { folderId: folder.id, name },
      { onSuccess: () => { setNewSubfolderName(''); setAddingSubfolder(false); } }
    );
  }

  return (
    <div
      onDragOver={(e) => {
        e.preventDefault();
        setDragOver(true);
      }}
      onDragLeave={() => setDragOver(false)}
      onDrop={(e) => {
        e.preventDefault();
        setDragOver(false);
        const eventId = e.dataTransfer.getData('text/plain');
        if (eventId) moveEventToFolder.mutate({ eventId, subfolderId: null });
      }}
      className="mb-1 rounded-md border border-dashed"
      style={{ borderColor: dragOver ? folder.colorBorder : 'transparent', backgroundColor: dragOver ? folder.colorBackground : undefined }}
    >
      <div className="flex items-center gap-2 rounded-md px-2 py-1.5">
        <input
          type="checkbox"
          checked={folder.isVisible}
          onChange={(e) => setVisibility.mutate({ folderId: folder.id, isVisible: e.target.checked })}
        />
        <button type="button" onClick={() => setExpanded((v) => !v)} className="text-xs text-gray-400">
          {expanded ? '▼' : '▶'}
        </button>
        <span
          className="h-2.5 w-2.5 flex-shrink-0 rounded-full"
          style={{ backgroundColor: folder.colorBorder }}
        />
        {renaming ? (
          <input
            autoFocus
            type="text"
            value={nameDraft}
            onChange={(e) => setNameDraft(e.target.value)}
            onBlur={commitRename}
            onKeyDown={(e) => {
              if (e.key === 'Enter') commitRename();
              if (e.key === 'Escape') { setNameDraft(folder.name); setRenaming(false); }
            }}
            className="flex-1 rounded border border-gray-300 px-1 py-0.5 text-sm"
          />
        ) : (
          <span
            className="flex-1 truncate text-sm font-medium"
            style={{ color: folder.colorText }}
            onDoubleClick={() => setRenaming(true)}
          >
            {folder.name}
          </span>
        )}
        <button
          type="button"
          onClick={() => setLock.mutate({ folderId: folder.id, isLocked: !folder.isLocked })}
          className="text-xs"
        >
          <LockIcon locked={folder.isLocked} />
        </button>
        <button
          type="button"
          onClick={() => {
            if (confirm(`Delete folder "${folder.name}" and its subfolders? Events won't be deleted, just un-filed.`)) {
              deleteFolder.mutate(folder.id);
            }
          }}
          className="text-xs text-gray-300 hover:text-red-500"
          aria-label={`Delete ${folder.name}`}
        >
          ✕
        </button>
      </div>

      <AnimatePresence>
        {expanded && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: 'auto', opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            transition={{ duration: 0.15 }}
            className="overflow-hidden pb-1"
          >
            {folder.subfolders.map((subfolder) => (
              <SubfolderRow key={subfolder.id} folder={folder} subfolder={subfolder} />
            ))}

            {addingSubfolder ? (
              <div className="ml-4 flex gap-1 px-2 py-1">
                <input
                  autoFocus
                  type="text"
                  value={newSubfolderName}
                  onChange={(e) => setNewSubfolderName(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') handleCreateSubfolder();
                    if (e.key === 'Escape') setAddingSubfolder(false);
                  }}
                  placeholder="Subfolder name"
                  className="flex-1 rounded border border-gray-300 px-1.5 py-0.5 text-xs"
                />
                <button type="button" onClick={handleCreateSubfolder} className="text-xs text-blue-500">
                  Add
                </button>
              </div>
            ) : (
              <button
                type="button"
                onClick={() => setAddingSubfolder(true)}
                className="ml-4 px-2 py-1 text-xs text-gray-400 hover:text-gray-600"
              >
                + Subfolder
              </button>
            )}
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}

export function FolderTree() {
  const folderTree = useFolderTree();
  const settings = useCalendarSettings();
  const createFolder = useCreateFolder();
  const updateSettings = useUpdateCalendarSettings();
  const [addingFolder, setAddingFolder] = useState(false);
  const [newFolderName, setNewFolderName] = useState('');
  const [showSettings, setShowSettings] = useState(false);

  function handleCreateFolder() {
    const name = newFolderName.trim();
    if (!name) return;
    createFolder.mutate(name, { onSuccess: () => { setNewFolderName(''); setAddingFolder(false); } });
  }

  function toggleMasterLock() {
    if (!settings.data) return;
    updateSettings.mutate({ ...settings.data, masterLockEnabled: !settings.data.masterLockEnabled });
  }

  return (
    <div className="w-72 flex-shrink-0 rounded-lg border border-gray-200 bg-white p-3 shadow-sm">
      <div className="mb-2 flex items-center justify-between">
        <h2 className="text-sm font-semibold text-gray-800">Calendar Folders</h2>
        <button
          type="button"
          onClick={() => setShowSettings((v) => !v)}
          className="text-xs text-gray-400 hover:text-gray-600"
          aria-label="Calendar settings"
        >
          ⚙
        </button>
      </div>

      <AnimatePresence>
        {showSettings && settings.data && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: 'auto', opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            className="mb-3 overflow-hidden rounded-md border border-gray-200 bg-gray-50 p-2 text-xs"
          >
            <label className="mb-1.5 flex items-center justify-between">
              <span>Lock new folders by default</span>
              <input
                type="checkbox"
                checked={settings.data.lockFoldersByDefault}
                onChange={(e) => updateSettings.mutate({ ...settings.data!, lockFoldersByDefault: e.target.checked })}
              />
            </label>
            <label className="mb-1.5 flex items-center justify-between">
              <span>Auto-recolor by time sensitivity</span>
              <input
                type="checkbox"
                checked={settings.data.autoRecolorByTimeSensitivity}
                onChange={(e) =>
                  updateSettings.mutate({ ...settings.data!, autoRecolorByTimeSensitivity: e.target.checked })
                }
              />
            </label>
            <label className="flex items-center justify-between">
              <span>Due-soon window (hours)</span>
              <input
                type="number"
                min={1}
                max={720}
                value={settings.data.dueSoonWindowHours}
                onChange={(e) =>
                  updateSettings.mutate({ ...settings.data!, dueSoonWindowHours: Number(e.target.value) })
                }
                className="w-16 rounded border border-gray-300 px-1 py-0.5"
              />
            </label>
          </motion.div>
        )}
      </AnimatePresence>

      <label className="mb-2 flex items-center gap-2 rounded-md bg-gray-50 px-2 py-1.5 text-sm">
        <LockIcon locked={settings.data?.masterLockEnabled ?? false} />
        <span className="flex-1">Master Lock</span>
        <input type="checkbox" checked={settings.data?.masterLockEnabled ?? false} onChange={toggleMasterLock} />
      </label>

      {folderTree.isLoading && <p className="text-xs text-gray-400">Loading folders...</p>}

      {folderTree.data?.folders.map((folder) => (
        <FolderRow key={folder.id} folder={folder} />
      ))}

      {addingFolder ? (
        <div className="mt-1 flex gap-1">
          <input
            autoFocus
            type="text"
            value={newFolderName}
            onChange={(e) => setNewFolderName(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') handleCreateFolder();
              if (e.key === 'Escape') setAddingFolder(false);
            }}
            placeholder="Folder name"
            className="flex-1 rounded border border-gray-300 px-1.5 py-1 text-xs"
          />
          <button type="button" onClick={handleCreateFolder} className="text-xs text-blue-500">
            Add
          </button>
        </div>
      ) : (
        <button
          type="button"
          onClick={() => setAddingFolder(true)}
          className="mt-1 w-full rounded-md border border-dashed border-gray-300 py-1.5 text-xs text-gray-400 hover:text-gray-600"
        >
          + New Folder
        </button>
      )}
    </div>
  );
}
