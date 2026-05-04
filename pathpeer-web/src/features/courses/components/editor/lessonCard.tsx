import React, { useState } from 'react';
import {
  DndContext,
//   DragEndEvent,
  PointerSensor,
  useSensor,
  useSensors,
  closestCenter,
} from '@dnd-kit/core';
import type {
  DragEndEvent,
} from '@dnd-kit/core';
import {
  SortableContext,
  verticalListSortingStrategy,
  useSortable,
  arrayMove,
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import type { Lesson, BlockType } from '../../../../shared/types/course';
import { useCourseStore } from '../../store/courseEditorStore';
import { BlockEditor } from './blockEditor';
import { ConfirmDialog } from './confirmDialog';

interface LessonCardProps {
  lesson: Lesson;
  sectionId: string;
}

const BLOCK_TYPES: { type: BlockType; label: string; icon: React.ReactNode }[] = [
  {
    type: 'Video',
    label: 'Video',
    icon: (
      <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
        <polygon points="5 3 19 12 5 21 5 3" />
      </svg>
    ),
  },
  {
    type: 'Text',
    label: 'Text',
    icon: (
      <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round">
        <line x1="4" y1="6" x2="20" y2="6" />
        <line x1="4" y1="12" x2="20" y2="12" />
        <line x1="4" y1="18" x2="14" y2="18" />
      </svg>
    ),
  },
  {
    type: 'File',
    label: 'File',
    icon: (
      <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z" />
        <polyline points="13 2 13 9 20 9" />
      </svg>
    ),
  },
];

export function LessonCard({ lesson, sectionId }: LessonCardProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: lesson.id });

  const {
    updateLessonTitle,
    toggleLesson,
    deleteLesson,
    addBlock,
    reorderBlocks,
  } = useCourseStore();

  const [confirmOpen, setConfirmOpen] = useState(false);
  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 4 } }));

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.45 : 1,
    zIndex: isDragging ? 10 : undefined,
  };

  const handleBlockDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over || active.id === over.id) return;
    const ids = lesson.blocks.map((b) => b.id);
    const oldIndex = ids.indexOf(active.id as string);
    const newIndex = ids.indexOf(over.id as string);
    reorderBlocks(sectionId, lesson.id, arrayMove(ids, oldIndex, newIndex));
  };

  return (
    <div ref={setNodeRef} style={style} className="bg-white rounded-xl border border-stone-200 overflow-hidden">
      {/* Lesson Header */}
      <div className="flex items-center gap-2 px-3 py-2.5 group">
        {/* Drag Handle */}
        <button
          {...attributes}
          {...listeners}
          className="p-0.5 text-stone-300 hover:text-stone-500 cursor-grab active:cursor-grabbing flex-shrink-0 rounded hover:bg-stone-100 transition-colors"
          tabIndex={-1}
          title="Drag to reorder"
        >
          <DragDots />
        </button>

        {/* Lesson Icon */}
        <span className="w-5 h-5 rounded-md bg-indigo-50 text-indigo-400 flex items-center justify-center flex-shrink-0">
          <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <polygon points="5 3 19 12 5 21 5 3" />
          </svg>
        </span>

        {/* Expand Toggle */}
        <button
          onClick={() => toggleLesson(sectionId, lesson.id)}
          className="p-0.5 text-stone-400 hover:text-stone-600 flex-shrink-0 transition-colors"
        >
          <ChevronIcon expanded={lesson.isExpanded} />
        </button>

        {/* Inline Title */}
        <input
          type="text"
          value={lesson.title}
          onChange={(e) => updateLessonTitle(sectionId, lesson.id, e.target.value)}
          onClick={(e) => e.stopPropagation()}
          placeholder="Lesson title…"
          className="flex-1 bg-transparent text-sm text-stone-700 placeholder-stone-300 focus:outline-none focus:ring-2 focus:ring-indigo-400 rounded-lg px-2 py-0.5 -mx-2 font-medium"
        />

        <span className="text-xs text-stone-400 flex-shrink-0 mr-1">
          {lesson.blocks.length} {lesson.blocks.length === 1 ? 'block' : 'blocks'}
        </span>

        {/* Delete */}
        <button
          onClick={() => setConfirmOpen(true)}
          className="p-1.5 text-stone-300 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors flex-shrink-0 opacity-0 group-hover:opacity-100"
          title="Delete lesson"
        >
          <TrashIcon size={13} />
        </button>
      </div>

      {/* Lesson Body — Blocks */}
      {lesson.isExpanded && (
        <div className="border-t border-stone-100 bg-stone-50/60 px-3 py-3">
          {/* Block Toolbar */}
          <div className="flex items-center gap-2 mb-3">
            <span className="text-xs text-stone-400 font-medium">Add:</span>
            {BLOCK_TYPES.map(({ type, label, icon }) => (
              <button
                key={type}
                onClick={() => addBlock(sectionId, lesson.id, type)}
                className="inline-flex items-center gap-1.5 px-2.5 py-1.5 bg-white border border-stone-200 hover:border-indigo-300 hover:text-indigo-600 hover:bg-indigo-50 rounded-lg text-xs font-medium text-stone-600 transition-all shadow-sm"
              >
                {icon}
                {label}
              </button>
            ))}
          </div>

          {/* Block List */}
          {lesson.blocks.length === 0 ? (
            <div className="py-5 text-center">
              <p className="text-xs text-stone-400">
                No content blocks yet. Use the toolbar above to add video, text, or files.
              </p>
            </div>
          ) : (
            <DndContext
              sensors={sensors}
              collisionDetection={closestCenter}
              onDragEnd={handleBlockDragEnd}
            >
              <SortableContext
                items={lesson.blocks.map((b) => b.id)}
                strategy={verticalListSortingStrategy}
              >
                <div className="flex flex-col gap-1.5">
                  {lesson.blocks.map((block) => (
                    <BlockEditor
                      key={block.id}
                      block={block}
                      sectionId={sectionId}
                      lessonId={lesson.id}
                    />
                  ))}
                </div>
              </SortableContext>
            </DndContext>
          )}
        </div>
      )}

      <ConfirmDialog
        open={confirmOpen}
        title="Delete Lesson"
        message={`Delete "${lesson.title}" and all its content blocks? This cannot be undone.`}
        onConfirm={() => { deleteLesson(sectionId, lesson.id); setConfirmOpen(false); }}
        onCancel={() => setConfirmOpen(false)}
      />
    </div>
  );
}

/* ─── Icons ─── */
function DragDots() {
  return (
    <svg width="13" height="13" viewBox="0 0 16 16" fill="currentColor">
      <circle cx="5.5" cy="3.5" r="1.1" />
      <circle cx="10.5" cy="3.5" r="1.1" />
      <circle cx="5.5" cy="8" r="1.1" />
      <circle cx="10.5" cy="8" r="1.1" />
      <circle cx="5.5" cy="12.5" r="1.1" />
      <circle cx="10.5" cy="12.5" r="1.1" />
    </svg>
  );
}

function ChevronIcon({ expanded }: { expanded: boolean }) {
  return (
    <svg
      width="14"
      height="14"
      viewBox="0 0 16 16"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      style={{ transform: expanded ? 'rotate(180deg)' : 'rotate(0deg)', transition: 'transform 0.2s ease' }}
    >
      <polyline points="4 6 8 10 12 6" />
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