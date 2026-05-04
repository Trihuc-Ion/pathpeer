import React, { useState } from 'react';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import type { Block, VideoBlockData, TextBlockData, FileBlockData } from '../../../../shared/types/course';
import { useCourseStore } from '../../store/courseEditorStore';
import { ConfirmDialog } from './confirmDialog';

interface BlockEditorProps {
  block: Block;
  sectionId: string;
  lessonId: string;
}

export function BlockEditor({ block, sectionId, lessonId }: BlockEditorProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } =
    useSortable({ id: block.id });
  const { updateBlock, deleteBlock } = useCourseStore();
  const [confirmOpen, setConfirmOpen] = useState(false);

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.45 : 1,
    zIndex: isDragging ? 10 : undefined,
  };

  const update = (data: Partial<Block['data']>) =>
    updateBlock(sectionId, lessonId, block.id, data);

  const typeConfig: Record<string, { label: string; bg: string; text: string; dot: string }> = {
    Video: { label: 'Video',  bg: 'bg-violet-50',  text: 'text-violet-700', dot: 'bg-violet-400' },
    Text:  { label: 'Text',   bg: 'bg-sky-50',     text: 'text-sky-700',    dot: 'bg-sky-400'    },
    File:  { label: 'File',   bg: 'bg-amber-50',   text: 'text-amber-700',  dot: 'bg-amber-400'  },
  };

  const cfg = typeConfig[block.type];

  return (
    <div
      ref={setNodeRef}
      style={style}
      className="bg-white rounded-xl border border-stone-200 overflow-hidden group"
    >
      <div className="flex items-start gap-3 p-3">
        {/* Drag Handle */}
        <button
          {...attributes}
          {...listeners}
          className="mt-0.5 p-1 text-stone-300 hover:text-stone-500 cursor-grab active:cursor-grabbing flex-shrink-0 rounded-lg hover:bg-stone-100 transition-colors"
          title="Drag to reorder"
          tabIndex={-1}
        >
          <DragDots />
        </button>

        {/* Type Badge */}
        <div className={`mt-0.5 flex items-center gap-1.5 px-2 py-1 rounded-lg ${cfg.bg} flex-shrink-0`}>
          <span className={`w-1.5 h-1.5 rounded-full ${cfg.dot} flex-shrink-0`} />
          <span className={`text-xs font-semibold ${cfg.text}`}>{cfg.label}</span>
        </div>

        {/* Editor */}
        <div className="flex-1 min-w-0">
          {block.type === 'Video' && (
            <VideoBlockEditor data={block.data as VideoBlockData} update={update} />
          )}
          {block.type === 'Text' && (
            <TextBlockEditor data={block.data as TextBlockData} update={update} />
          )}
          {block.type === 'File' && (
            <FileBlockEditor data={block.data as FileBlockData} update={update} />
          )}
        </div>

        {/* Delete */}
        <button
          onClick={() => setConfirmOpen(true)}
          className="mt-0.5 p-1.5 text-stone-300 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors flex-shrink-0 opacity-0 group-hover:opacity-100"
          title="Delete block"
        >
          <TrashIcon size={14} />
        </button>
      </div>

      <ConfirmDialog
        open={confirmOpen}
        title="Delete Block"
        message={`Delete this ${block.type.toLowerCase()} block? This action cannot be undone.`}
        onConfirm={() => { deleteBlock(sectionId, lessonId, block.id); setConfirmOpen(false); }}
        onCancel={() => setConfirmOpen(false)}
      />
    </div>
  );
}

/* ─── Video Block Editor ─── */
function VideoBlockEditor({
  data,
  update,
}: {
  data: VideoBlockData;
  update: (d: Partial<VideoBlockData>) => void;
}) {
  return (
    <div className="flex flex-col gap-2">
      <select
        value={data.sourceType}
        onChange={(e) => update({ sourceType: e.target.value as 'Url' | 'File' })}
        className="w-36 rounded-lg border border-stone-200 px-2.5 py-1.5 text-xs text-stone-700 bg-white focus:outline-none focus:ring-2 focus:ring-indigo-400 transition"
      >
        <option value="Url">Video URL</option>
        <option value="File">Upload File</option>
      </select>

      {data.sourceType === 'Url' ? (
        <input
          type="url"
          placeholder="https://example.com/video.mp4"
          value={data.url || ''}
          onChange={(e) => update({ url: e.target.value })}
          className="w-full rounded-lg border border-stone-200 px-3 py-1.5 text-xs text-stone-700 placeholder-stone-400 focus:outline-none focus:ring-2 focus:ring-indigo-400 transition"
        />
      ) : (
        <label className="flex items-center gap-2.5 cursor-pointer">
          <span className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-stone-100 hover:bg-stone-200 rounded-lg text-xs text-stone-600 font-medium transition-colors">
            <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
              <polyline points="17 8 12 3 7 8"/>
              <line x1="12" y1="3" x2="12" y2="15"/>
            </svg>
            Choose Video
          </span>
          <span className="text-xs text-stone-400 truncate max-w-[180px]">
            {data.fileName || 'No file chosen'}
          </span>
          <input
            type="file"
            accept="video/*"
            className="hidden"
            onChange={(e) => update({ fileName: e.target.files?.[0]?.name || '' })}
          />
        </label>
      )}
    </div>
  );
}

/* ─── Text Block Editor ─── */
function TextBlockEditor({
  data,
  update,
}: {
  data: TextBlockData;
  update: (d: Partial<TextBlockData>) => void;
}) {
  return (
    <textarea
      rows={3}
      placeholder="Enter your text content here…"
      value={data.content}
      onChange={(e) => update({ content: e.target.value })}
      className="w-full rounded-lg border border-stone-200 px-3 py-2 text-xs text-stone-700 placeholder-stone-400 focus:outline-none focus:ring-2 focus:ring-indigo-400 transition resize-none leading-relaxed"
    />
  );
}

/* ─── File Block Editor ─── */
function FileBlockEditor({
  data,
  update,
}: {
  data: FileBlockData;
  update: (d: Partial<FileBlockData>) => void;
}) {
  return (
    <label className="flex items-center gap-2.5 cursor-pointer">
      <span className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-stone-100 hover:bg-stone-200 rounded-lg text-xs text-stone-600 font-medium transition-colors">
        <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
          <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
          <polyline points="17 8 12 3 7 8"/>
          <line x1="12" y1="3" x2="12" y2="15"/>
        </svg>
        Choose File
      </span>
      {data.fileName ? (
        <span className="flex items-center gap-1.5 text-xs text-stone-600 bg-stone-50 px-2.5 py-1.5 rounded-lg border border-stone-200">
          <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/>
            <polyline points="13 2 13 9 20 9"/>
          </svg>
          {data.fileName}
        </span>
      ) : (
        <span className="text-xs text-stone-400">No file chosen</span>
      )}
      <input
        type="file"
        className="hidden"
        onChange={(e) => update({ fileName: e.target.files?.[0]?.name || '' })}
      />
    </label>
  );
}

/* ─── Icons ─── */
function DragDots() {
  return (
    <svg width="14" height="14" viewBox="0 0 16 16" fill="currentColor">
      <circle cx="5.5" cy="3.5" r="1.1" />
      <circle cx="10.5" cy="3.5" r="1.1" />
      <circle cx="5.5" cy="8" r="1.1" />
      <circle cx="10.5" cy="8" r="1.1" />
      <circle cx="5.5" cy="12.5" r="1.1" />
      <circle cx="10.5" cy="12.5" r="1.1" />
    </svg>
  );
}

function TrashIcon({ size = 16 }: { size?: number }) {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <polyline points="3 6 5 6 21 6" />
      <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
    </svg>
  );
}