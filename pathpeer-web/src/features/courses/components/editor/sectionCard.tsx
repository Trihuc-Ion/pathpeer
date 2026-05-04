import React, { useState } from 'react';
import {
  DndContext,
//   DragEndEvent,
  PointerSensor,
  useSensor,
  useSensors,
  closestCenter,
} from '@dnd-kit/core';
import type{
  DragEndEvent,
} from '@dnd-kit/core';
import {
  SortableContext,
  verticalListSortingStrategy,
  useSortable,
  arrayMove,
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import type { Section } from '../../../../shared/types/course';
import { useCourseStore } from '../../store/courseEditorStore';
import { LessonCard } from './lessonCard';
import { ConfirmDialog } from './confirmDialog';

interface SectionCardProps {
  section: Section;
}

export function SectionCard({ section }: SectionCardProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: section.id });

  const {
    updateSectionTitle,
    toggleSection,
    deleteSection,
    addLesson,
    reorderLessons,
  } = useCourseStore();

  const [confirmOpen, setConfirmOpen] = useState(false);
  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 4 } }));

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.45 : 1,
    zIndex: isDragging ? 20 : undefined,
  };

  const handleLessonDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over || active.id === over.id) return;
    const ids = section.lessons.map((l) => l.id);
    const oldIndex = ids.indexOf(active.id as string);
    const newIndex = ids.indexOf(over.id as string);
    reorderLessons(section.id, arrayMove(ids, oldIndex, newIndex));
  };

  const totalBlocks = section.lessons.reduce((sum, l) => sum + l.blocks.length, 0);

  return (
    <div
      ref={setNodeRef}
      style={style}
      className="bg-white rounded-2xl border border-stone-200 shadow-sm overflow-hidden"
    >
      {/* Section Header */}
      <div className="flex items-center gap-2.5 px-4 py-3 bg-gradient-to-r from-stone-50 to-stone-50/60 border-b border-stone-100 group">
        {/* Drag Handle */}
        <button
          {...attributes}
          {...listeners}
          className="p-1 text-stone-300 hover:text-stone-500 cursor-grab active:cursor-grabbing flex-shrink-0 rounded-lg hover:bg-stone-100 transition-colors"
          tabIndex={-1}
          title="Drag to reorder section"
        >
          <DragDots />
        </button>

        {/* Section index badge */}
        <span className="w-6 h-6 rounded-md bg-indigo-100 text-indigo-600 text-xs font-bold flex items-center justify-center flex-shrink-0">
          {/* We derive the index from the order — just show a section icon */}
          <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <rect x="3" y="3" width="7" height="7" rx="1" />
            <rect x="14" y="3" width="7" height="7" rx="1" />
            <rect x="3" y="14" width="7" height="7" rx="1" />
            <rect x="14" y="14" width="7" height="7" rx="1" />
          </svg>
        </span>

        {/* Expand Toggle */}
        <button
          onClick={() => toggleSection(section.id)}
          className="p-0.5 text-stone-400 hover:text-stone-600 flex-shrink-0 transition-colors"
        >
          <ChevronIcon expanded={section.isExpanded} />
        </button>

        {/* Inline Title */}
        <input
          type="text"
          value={section.title}
          onChange={(e) => updateSectionTitle(section.id, e.target.value)}
          onClick={(e) => e.stopPropagation()}
          placeholder="Section title…"
          className="flex-1 bg-transparent text-sm font-semibold text-stone-800 placeholder-stone-300 focus:outline-none focus:ring-2 focus:ring-indigo-400 rounded-lg px-2 py-1 -mx-2"
        />

        {/* Stats */}
        <div className="flex items-center gap-3 flex-shrink-0 mr-1">
          <span className="text-xs text-stone-400 hidden sm:inline">
            {section.lessons.length} {section.lessons.length === 1 ? 'lesson' : 'lessons'}
          </span>
          {totalBlocks > 0 && (
            <span className="text-xs text-stone-400 hidden sm:inline">
              · {totalBlocks} blocks
            </span>
          )}
        </div>

        {/* Add Lesson */}
        <button
          onClick={() => addLesson(section.id)}
          className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-semibold text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors flex-shrink-0"
        >
          <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <line x1="12" y1="5" x2="12" y2="19" />
            <line x1="5" y1="12" x2="19" y2="12" />
          </svg>
          Lesson
        </button>

        {/* Delete Section */}
        <button
          onClick={() => setConfirmOpen(true)}
          className="p-1.5 text-stone-300 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors flex-shrink-0 opacity-0 group-hover:opacity-100"
          title="Delete section"
        >
          <TrashIcon size={14} />
        </button>
      </div>

      {/* Lessons */}
      {section.isExpanded && (
        <div className="p-3">
          {section.lessons.length === 0 ? (
            <div className="rounded-xl border border-dashed border-stone-200 py-8 text-center">
              <div className="w-8 h-8 rounded-lg bg-stone-100 flex items-center justify-center mx-auto mb-2">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#a8a29e" strokeWidth="2">
                  <polygon points="5 3 19 12 5 21 5 3" />
                </svg>
              </div>
              <p className="text-xs text-stone-400 mb-2">No lessons in this section</p>
              <button
                onClick={() => addLesson(section.id)}
                className="text-xs text-indigo-600 font-semibold hover:underline"
              >
                + Add first lesson
              </button>
            </div>
          ) : (
            <DndContext
              sensors={sensors}
              collisionDetection={closestCenter}
              onDragEnd={handleLessonDragEnd}
            >
              <SortableContext
                items={section.lessons.map((l) => l.id)}
                strategy={verticalListSortingStrategy}
              >
                <div className="flex flex-col gap-2">
                  {section.lessons.map((lesson) => (
                    <LessonCard
                      key={lesson.id}
                      lesson={lesson}
                      sectionId={section.id}
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
        title="Delete Section"
        message={`Delete "${section.title}" along with all ${section.lessons.length} lesson(s) inside? This cannot be undone.`}
        onConfirm={() => { deleteSection(section.id); setConfirmOpen(false); }}
        onCancel={() => setConfirmOpen(false)}
      />
    </div>
  );
}

/* ─── Icons ─── */
function DragDots() {
  return (
    <svg width="15" height="15" viewBox="0 0 16 16" fill="currentColor">
      <circle cx="5.5" cy="3.5" r="1.15" />
      <circle cx="10.5" cy="3.5" r="1.15" />
      <circle cx="5.5" cy="8" r="1.15" />
      <circle cx="10.5" cy="8" r="1.15" />
      <circle cx="5.5" cy="12.5" r="1.15" />
      <circle cx="10.5" cy="12.5" r="1.15" />
    </svg>
  );
}

function ChevronIcon({ expanded }: { expanded: boolean }) {
  return (
    <svg
      width="16"
      height="16"
      viewBox="0 0 16 16"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      style={{
        transform: expanded ? 'rotate(180deg)' : 'rotate(0deg)',
        transition: 'transform 0.2s ease',
      }}
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