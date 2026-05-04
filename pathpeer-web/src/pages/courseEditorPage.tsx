import React, { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  DndContext,
  //   DragEndEvent,
  PointerSensor,
  useSensor,
  useSensors,
  closestCenter,
} from '@dnd-kit/core';
import type {
  DragEndEvent
} from '@dnd-kit/core';
import {
  SortableContext,
  verticalListSortingStrategy,
  arrayMove,
} from '@dnd-kit/sortable';
import { useCourseStore, isLocalId } from '../features/courses/store/courseEditorStore';
import { courseApi } from '../features/courses/api/courseApi';
import { SectionCard } from '../features/courses/components/editor/sectionCard';
import type { Block, CourseLevel } from '../shared/types/course';
import { useQuery } from '@tanstack/react-query';
import type { Course } from '../shared/types';

const LANGUAGES = ['English', 'Romanian', 'French', 'German', 'Spanish', 'Italian', 'Portuguese', 'Japanese', 'Chinese'];
const LEVELS: CourseLevel[] = ['Beginner', 'Intermediate', 'Advanced'];
const LEVEL_META: Record<CourseLevel, { color: string; bg: string }> = {
  Beginner: { color: 'text-emerald-700', bg: 'bg-emerald-50' },
  Intermediate: { color: 'text-amber-700', bg: 'bg-amber-50' },
  Advanced: { color: 'text-red-700', bg: 'bg-red-50' },
};

type SaveStatus = 'idle' | 'saving' | 'success' | 'error';

export default function CoursePage() {
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);
  const navigate = useNavigate();

  const { course, initCreate, setCourse, updateCourseField, addSection, reorderSections } =
    useCourseStore();
  const localIds = useCourseStore((s) => s.localIds);

  const [saveStatus, setSaveStatus] = useState<SaveStatus>('idle');
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveProgress, setSaveProgress] = useState<string>('');

  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 6 } }));

  const { data, isLoading, error: loadError, isSuccess} = useQuery({
    queryKey: ['course', id],
    queryFn: () => courseApi.getCourse(id!),
    enabled: isEdit && Boolean(id),
    staleTime: Infinity, 
  });

  useEffect(() => {
    if (isSuccess && data) {
      setCourse(data);
    }
  }, [isSuccess, data]);

  useEffect(() => {
    if (!isEdit) initCreate();
  }, []); 

  const handleSectionDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (!over || active.id === over.id) return;
    const ids = sections.map((s) => s.id);
    reorderSections(arrayMove(ids, ids.indexOf(active.id as string), ids.indexOf(over.id as string)));
  };

  const handleSave = useCallback(async (publishForReview = false) => {
    setSaveStatus('saving');
    setSaveError(null);

    try {
      let courseId: string;

      if (isEdit && !isLocalId(course.id)) {
        courseId = course.id;
        setSaveProgress('Updating course…');
      } else {
        setSaveProgress('Creating course…');
        courseId = await courseApi.createCourse({
          title: course.title || 'Untitled Course',
          description: course.description,
          language: course.language,
          price: course.price,
          level: course.level,
        });
      }
      
      for (const section of course.sections) {
        let sectionId: string;

        if (!isLocalId(section.id)) {
          sectionId = section.id; 
        } else {
          setSaveProgress(`Saving section "${section.title}"…`);
          sectionId = await courseApi.createSection(courseId, {
            title: section.title,
            order: section.order,
          });
        }

        for (const lesson of section.lessons) {
          let lessonId: string;

          if (!isLocalId(lesson.id)) {
            lessonId = lesson.id; 
          } else {
            setSaveProgress(`Saving lesson "${lesson.title}"…`);
            lessonId = await courseApi.createLesson(sectionId, {
              title: lesson.title,
              order: lesson.order,
            });
          }

          for (const block of lesson.blocks) {
            if (!isLocalId(block.id)) continue; 

            setSaveProgress(`Saving ${block.type.toLowerCase()} block…`);
            await courseApi.createBlock(lessonId, {
              type: block.type,
              order: block.order,
              data: block.data as Record<string, unknown>,
            });
          }
        }
      }

      setSaveProgress('');
      setSaveStatus('success');

      if (!isEdit) {
        navigate(`/courses/${courseId}/edit`, { replace: true });
      } else {
        const updated = await courseApi.getCourse(courseId);
        setCourse(updated);
      }

      if (publishForReview) {
        console.log('[PathPeer] Published for review — add PATCH endpoint when available.');
      }

      setTimeout(() => setSaveStatus('idle'), 3000);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Unknown error';
      setSaveError(msg);
      setSaveStatus('error');
      setSaveProgress('');
    }
  }, [course, isEdit, navigate, setCourse]);

  if (isEdit && isLoading) {
    return (
      <div className="min-h-screen bg-stone-50 flex items-center justify-center" style={{ fontFamily: "'Outfit', sans-serif" }}>
        <div className="text-center">
          <div className="w-10 h-10 border-2 border-indigo-300 border-t-indigo-600 rounded-full animate-spin mx-auto mb-4" />
          <p className="text-sm text-stone-500">Loading course…</p>
        </div>
      </div>
    );
  }

  if (isEdit && loadError) {
    return (
      <div className="min-h-screen bg-stone-50 flex items-center justify-center" style={{ fontFamily: "'Outfit', sans-serif" }}>
        <div className="bg-white rounded-2xl border border-red-200 p-8 max-w-sm text-center shadow-sm">
          <div className="w-10 h-10 rounded-xl bg-red-50 flex items-center justify-center mx-auto mb-4">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#ef4444" strokeWidth="2"><circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" /></svg>
          </div>
          <h3 className="font-semibold text-stone-800 mb-1">Failed to load course</h3>
          <p className="text-xs text-stone-500 mb-4">
            {(loadError as Error)?.message ?? 'Could not fetch course from server.'}
          </p>
          <button onClick={() => navigate('/courses')} className="text-sm text-indigo-600 font-medium hover:underline">
            ← Back to courses
          </button>
        </div>
      </div>
    );
  }
  
  const sections = course?.sections ?? [];

  const totalLessons = sections.reduce(
    (sum, s) => sum + (s.lessons?.length ?? 0),
    0
  );
  const newItemsCount = [...localIds].filter(id => !id.endsWith(course.id)).length;

  return (
    <div className="min-h-screen bg-stone-50" style={{ fontFamily: "'Outfit', sans-serif" }}>

      {/* ── Header ── */}
      <header className="bg-white border-b border-stone-200 sticky top-0 z-40 shadow-sm shadow-stone-100">
        <div className="max-w-5xl mx-auto px-5 sm:px-8 h-14 flex items-center justify-between gap-4">
          <div className="flex items-center gap-3 flex-shrink-0">
            <div className="w-7 h-7 rounded-lg bg-indigo-600 flex items-center justify-center shadow-sm">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.5">
                <path d="M12 2L2 7l10 5 10-5-10-5z" />
                <path d="M2 17l10 5 10-5" />
                <path d="M2 12l10 5 10-5" />
              </svg>
            </div>
            <span className="font-semibold text-stone-800 text-sm tracking-tight">PathPeer</span>
            <span className="text-stone-300 text-xs hidden sm:inline">›</span>
            <span className="text-stone-500 text-xs hidden sm:inline">{isEdit ? 'Edit Course' : 'Create Course'}</span>
          </div>

          <div className="flex items-center gap-3">
            {newItemsCount > 0 && saveStatus === 'idle' && (
              <span className="flex items-center gap-1.5 text-xs text-amber-600">
                <span className="w-1.5 h-1.5 rounded-full bg-amber-400" />
                {newItemsCount} unsaved change{newItemsCount !== 1 ? 's' : ''}
              </span>
            )}
            <div className="flex items-center gap-2 px-3 py-1.5 bg-amber-50 border border-amber-200 rounded-full">
              <span className="w-1.5 h-1.5 rounded-full bg-amber-400 animate-pulse" />
              <span className="text-xs font-medium text-amber-700">Draft</span>
            </div>
            {course.version > 1 && (
              <span className="text-xs text-stone-400 hidden sm:inline">v{course.version}</span>
            )}
          </div>
        </div>
      </header>

      <main className="max-w-5xl mx-auto px-5 sm:px-8 py-8 pb-28">

        <div className="mb-7">
          <h1 className="text-xl font-bold text-stone-900 tracking-tight">
            {isEdit ? 'Edit Course' : 'Create New Course'}
          </h1>
          <p className="text-sm text-stone-500 mt-0.5">
            {isEdit
              ? 'Changes to existing sections/lessons require the PUT endpoint (coming soon). You can add new items and save them.'
              : 'Fill in the details and build your curriculum, then save.'}
          </p>
        </div>

        <section className="bg-white rounded-2xl border border-stone-200 shadow-sm p-6 mb-6">
          <div className="flex items-center gap-2 mb-5">
            <span className="w-1 h-5 rounded-full bg-indigo-500 flex-shrink-0" />
            <h2 className="text-xs font-semibold uppercase tracking-widest text-stone-500">Course Details</h2>
          </div>

          <div className="grid grid-cols-1 gap-5">
            <div>
              <label className="block text-sm font-medium text-stone-700 mb-1.5">
                Title <span className="text-red-400">*</span>
              </label>
              <input
                type="text"
                value={course.title}
                onChange={(e) => updateCourseField('title', e.target.value)}
                placeholder="e.g. Complete Web Development Bootcamp"
                className="w-full rounded-xl border border-stone-200 px-4 py-2.5 text-stone-900 text-sm placeholder-stone-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-shadow"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-stone-700 mb-1.5">Description</label>
              <textarea
                rows={3}
                value={course.description}
                onChange={(e) => updateCourseField('description', e.target.value)}
                placeholder="What will students learn? Who is this course for?"
                className="w-full rounded-xl border border-stone-200 px-4 py-2.5 text-stone-900 text-sm placeholder-stone-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-shadow resize-none leading-relaxed"
              />
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <div>
                <label className="block text-sm font-medium text-stone-700 mb-1.5">Language</label>
                <div className="relative">
                  <select
                    value={course.language}
                    onChange={(e) => updateCourseField('language', e.target.value)}
                    className="w-full appearance-none rounded-xl border border-stone-200 px-4 py-2.5 text-stone-900 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-indigo-500 pr-9"
                  >
                    {LANGUAGES.map((l) => <option key={l} value={l}>{l}</option>)}
                  </select>
                  <SelectChevron />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-stone-700 mb-1.5">
                  Price <span className="text-stone-400 font-normal">(USD)</span>
                </label>
                <div className="relative">
                  <span className="absolute left-3.5 top-1/2 -translate-y-1/2 text-stone-400 text-sm pointer-events-none">$</span>
                  <input
                    type="number"
                    min={0}
                    step={0.01}
                    value={course.price || ''}
                    onChange={(e) => updateCourseField('price', parseFloat(e.target.value) || 0)}
                    placeholder="0.00"
                    className="w-full rounded-xl border border-stone-200 pl-7 pr-4 py-2.5 text-stone-900 text-sm placeholder-stone-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-stone-700 mb-1.5">Level</label>
                <div className="relative">
                  <select
                    value={course.level}
                    onChange={(e) => updateCourseField('level', e.target.value as CourseLevel)}
                    className="w-full appearance-none rounded-xl border border-stone-200 px-4 py-2.5 text-stone-900 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-indigo-500 pr-9"
                  >
                    {LEVELS.map((l) => <option key={l} value={l}>{l}</option>)}
                  </select>
                  <SelectChevron />
                </div>
                <span className={`inline-flex mt-1.5 px-2 py-0.5 rounded-full text-xs font-medium ${LEVEL_META[course.level].bg} ${LEVEL_META[course.level].color}`}>
                  {course.level}
                </span>
              </div>
            </div>
          </div>
        </section>

        {/* ── Curriculum ── */}
        <div className="flex items-center justify-between mb-4">
          <div>
            <h2 className="text-base font-bold text-stone-900">Curriculum</h2>
            <p className="text-xs text-stone-500 mt-0.5">
              {sections.length} section{sections.length !== 1 ? 's' : ''}
              {totalLessons > 0 && ` · ${totalLessons} lesson${totalLessons !== 1 ? 's' : ''}`}
            </p>
          </div>
          <button
            onClick={addSection}
            className="flex items-center gap-2 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-sm font-semibold transition-colors shadow-sm shadow-indigo-200"
          >
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <line x1="12" y1="5" x2="12" y2="19" />
              <line x1="5" y1="12" x2="19" y2="12" />
            </svg>
            Add Section
          </button>
        </div>

        <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleSectionDragEnd}>
          <SortableContext items={sections.map((s) => s.id)} strategy={verticalListSortingStrategy}>
            <div className="flex flex-col gap-3">
              {sections.map((section) => (
                <SectionCard key={section.id} section={section} />
              ))}
            </div>
          </SortableContext>
        </DndContext>

        {sections.length === 0 && (
          <div className="bg-white rounded-2xl border border-dashed border-stone-300 py-16 text-center">
            <div className="w-14 h-14 rounded-2xl bg-indigo-50 flex items-center justify-center mx-auto mb-4">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#6366f1" strokeWidth="1.5">
                <path d="M12 2L2 7l10 5 10-5-10-5z" />
                <path d="M2 17l10 5 10-5" />
                <path d="M2 12l10 5 10-5" />
              </svg>
            </div>
            <h3 className="font-semibold text-stone-700 mb-1.5">No sections yet</h3>
            <p className="text-sm text-stone-400 mb-5 max-w-xs mx-auto">
              Start building your curriculum by adding sections and lessons.
            </p>
            <button
              onClick={addSection}
              className="px-5 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-sm font-semibold transition-colors shadow-sm"
            >
              + Add First Section
            </button>
          </div>
        )}
      </main>

      {/* ── Sticky Footer ── */}
      <footer className="fixed bottom-0 left-0 right-0 bg-white/95 backdrop-blur border-t border-stone-200 z-40">
        <div className="max-w-5xl mx-auto px-5 sm:px-8 h-16 flex items-center justify-between gap-4">

          {/* Status area */}
          <div className="flex items-center gap-2 min-w-0 flex-1">
            {saveStatus === 'saving' && (
              <div className="flex items-center gap-2 text-xs text-indigo-600">
                <div className="w-3.5 h-3.5 border-2 border-indigo-300 border-t-indigo-600 rounded-full animate-spin flex-shrink-0" />
                <span className="truncate">{saveProgress || 'Saving…'}</span>
              </div>
            )}
            {saveStatus === 'success' && (
              <span className="flex items-center gap-1.5 text-xs text-emerald-600 font-medium">
                <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                  <polyline points="20 6 9 17 4 12" />
                </svg>
                Saved successfully
              </span>
            )}
            {saveStatus === 'error' && (
              <span className="flex items-center gap-1.5 text-xs text-red-600 font-medium truncate">
                <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                  <circle cx="12" cy="12" r="10" />
                  <line x1="12" y1="8" x2="12" y2="12" />
                  <line x1="12" y1="16" x2="12.01" y2="16" />
                </svg>
                {saveError ?? 'Save failed'}
              </span>
            )}
            {saveStatus === 'idle' && (
              <p className="text-xs text-stone-400 truncate">
                {newItemsCount > 0
                  ? `${newItemsCount} unsaved change${newItemsCount !== 1 ? 's' : ''}`
                  : isEdit ? 'All changes saved' : 'Ready to save'}
              </p>
            )}
          </div>

          {/* Buttons */}
          <div className="flex items-center gap-2.5 flex-shrink-0">
            <button
              onClick={() => handleSave(false)}
              disabled={saveStatus === 'saving'}
              className="px-4 py-2 rounded-xl border border-stone-200 text-stone-700 hover:bg-stone-50 active:bg-stone-100 text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Save Draft
            </button>
            <button
              onClick={() => handleSave(true)}
              disabled={saveStatus === 'saving'}
              className="flex items-center gap-2 px-5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-700 active:bg-indigo-800 text-white text-sm font-semibold transition-colors shadow-sm shadow-indigo-200 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <path d="M22 2L11 13" />
                <path d="M22 2L15 22 11 13 2 9l20-7z" />
              </svg>
              Publish for Review
            </button>
          </div>
        </div>
      </footer>
    </div>
  );
}

function SelectChevron() {
  return (
    <div className="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-stone-400">
      <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round">
        <polyline points="4 6 8 10 12 6" />
      </svg>
    </div>
  );
}