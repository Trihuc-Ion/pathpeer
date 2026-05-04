import React, { useEffect, useRef, useState, useCallback } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { courseApi } from '../features/courses/api/courseApi';
import { getYouTubeEmbedUrl, getYouTubeThumbnail } from '../shared/utils/youtube';
import { useAuthStore } from '../features/auth/store/authStore';
import type { Course, Section, Lesson, Block } from '../shared/types/course';
import type { VideoBlockData, TextBlockData, FileBlockData } from '../shared/types/course';

/* ─────────────────────────────────────────────────────────
   MAIN PAGE
   ───────────────────────────────────────────────────────── */
export default function CourseViewPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const currentUserId = useAuthStore((s) => s.user?.id); 
  const [activeLessonId, setActiveLessonId] = useState<string | null>(null);
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const lessonRefs = useRef<Map<string, HTMLElement>>(new Map());
  const observerRef = useRef<IntersectionObserver | null>(null);

  const { data: course, isLoading, error } = useQuery({
    queryKey: ['course-view', id],
    queryFn: () => courseApi.getCourse(id!),
    enabled: Boolean(id),
  });

  /* ── Flatten all lessons for easy navigation ── */
  const allLessons: { lesson: Lesson; sectionTitle: string }[] = (course?.sections ?? []).flatMap(
    (s) => s.lessons.map((l) => ({ lesson: l, sectionTitle: s.title }))
  );

  /* ── IntersectionObserver — highlights active lesson in sidebar ── */
  useEffect(() => {
    if (!allLessons.length) return;

    observerRef.current?.disconnect();

    observerRef.current = new IntersectionObserver(
      (entries) => {
        const visible = entries
          .filter((e) => e.isIntersecting)
          .sort((a, b) => a.boundingClientRect.top - b.boundingClientRect.top);
        if (visible[0]) {
          setActiveLessonId(visible[0].target.id.replace('lesson-', ''));
        }
      },
      { threshold: 0.25, rootMargin: '-80px 0px -60% 0px' }
    );

    lessonRefs.current.forEach((el) => observerRef.current?.observe(el));

    return () => observerRef.current?.disconnect();
  }, [allLessons.length]);

  const registerRef = useCallback((lessonId: string, el: HTMLElement | null) => {
    if (el) {
      lessonRefs.current.set(lessonId, el);
      observerRef.current?.observe(el);
    }
  }, []);

  const scrollToLesson = (lessonId: string) => {
    const el = lessonRefs.current.get(lessonId);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
      setSidebarOpen(false);
    }
  };

  /* ── Set first lesson active on load ── */
  useEffect(() => {
    if (allLessons.length && !activeLessonId) {
      setActiveLessonId(allLessons[0].lesson.id);
    }
  }, [allLessons.length]);

  /* ── Loading ── */
  if (isLoading) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center" style={font}>
        <div className="text-center">
          <div className="w-10 h-10 border-2 border-slate-600 border-t-indigo-400 rounded-full animate-spin mx-auto mb-4" />
          <p className="text-sm text-slate-400">Loading course…</p>
        </div>
      </div>
    );
  }

  /* ── Error ── */
  if (error || !course) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center" style={font}>
        <div className="text-center">
          <p className="text-slate-300 mb-4">Could not load course.</p>
          <button onClick={() => navigate(-1)} className="text-indigo-400 hover:underline text-sm">
            ← Go back
          </button>
        </div>
      </div>
    );
  }

  const totalLessons = allLessons.length;
  const totalSections = course.sections.length;

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col" style={font}>

      {/* ── Top bar ── */}
            <header className="h-14 bg-slate-900 border-b border-slate-800 flex items-center px-4 gap-4 sticky top-0 z-50">
        <button
          onClick={() => navigate(-1)}
          className="text-slate-400 hover:text-white transition-colors flex items-center gap-1.5 text-sm"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round">
            <polyline points="15 18 9 12 15 6" />
          </svg>
          <span className="hidden sm:inline">Back</span>
        </button>
 
        <div className="w-px h-5 bg-slate-700" />
 
        <div className="flex items-center gap-2 flex-1 min-w-0">
          <div className="w-6 h-6 rounded-md bg-indigo-600 flex items-center justify-center flex-shrink-0">
            <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.5">
              <path d="M12 2L2 7l10 5 10-5-10-5z" /><path d="M2 17l10 5 10-5" /><path d="M2 12l10 5 10-5" />
            </svg>
          </div>
          <span className="text-slate-200 text-sm font-medium truncate">{course.title}</span>
        </div>
 
        {/* Edit button — only for course creator */}
        {currentUserId === course.creatorId && (
          <Link
            to={`/courses/${id}/edit`}
            className="flex items-center gap-1.5 px-3 py-1.5 bg-slate-700 hover:bg-slate-600 text-slate-200 hover:text-white rounded-lg text-xs font-medium transition-colors flex-shrink-0"
          >
            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
              <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
            </svg>
            Edit course
          </Link>
        )}
 
        {/* Mobile sidebar toggle */}
        <button
          onClick={() => setSidebarOpen(!sidebarOpen)}
          className="lg:hidden p-2 text-slate-400 hover:text-white"
        >
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <line x1="3" y1="6" x2="21" y2="6" /><line x1="3" y1="12" x2="21" y2="12" /><line x1="3" y1="18" x2="21" y2="18" />
          </svg>
        </button>
      </header>


      <div className="flex flex-1 overflow-hidden">

        {/* ── Sidebar overlay (mobile) ── */}
        {sidebarOpen && (
          <div
            className="fixed inset-0 bg-black/50 z-30 lg:hidden"
            onClick={() => setSidebarOpen(false)}
          />
        )}

        {/* ── Sidebar ── */}
        <aside className={`
          fixed lg:sticky top-14 h-[calc(100vh-3.5rem)] w-72 bg-slate-900 border-r border-slate-800
          flex flex-col z-40 transition-transform duration-300 overflow-hidden
          ${sidebarOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'}
        `}>
          {/* Sidebar header */}
          <div className="p-5 border-b border-slate-800 flex-shrink-0">
            <h2 className="text-white font-semibold text-sm leading-snug line-clamp-2 mb-3">
              {course.title}
            </h2>
            <div className="flex items-center gap-3 text-xs text-slate-500">
              <span className="flex items-center gap-1">
                <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <rect x="3" y="3" width="7" height="7" rx="1" /><rect x="14" y="3" width="7" height="7" rx="1" />
                  <rect x="3" y="14" width="7" height="7" rx="1" /><rect x="14" y="14" width="7" height="7" rx="1" />
                </svg>
                {totalSections} sections
              </span>
              <span className="flex items-center gap-1">
                <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <polygon points="5 3 19 12 5 21 5 3" />
                </svg>
                {totalLessons} lessons
              </span>
              <LevelBadge level={course.level} />
            </div>
          </div>

          {/* Section + Lesson list */}
          <nav className="flex-1 overflow-y-auto py-2 scrollbar-thin">
            {course.sections.map((section, si) => (
              <SidebarSection
                key={section.id}
                section={section}
                sectionIndex={si}
                activeLessonId={activeLessonId}
                onLessonClick={scrollToLesson}
              />
            ))}
          </nav>
        </aside>

        {/* ── Main content ── */}
        <main className="flex-1 overflow-y-auto lg:ml-0">
          <div className="max-w-3xl mx-auto px-5 sm:px-8 py-10">

            {/* Course hero */}
            <div className="mb-12">
              <div className="flex items-center gap-2 mb-3">
                <LevelBadge level={course.level} />
                <span className="text-slate-400 text-xs">{course.language}</span>
                {/* <span className="text-slate-400 text-xs">{course.status}</span> */}
              </div>
              <h1 className="text-3xl font-bold text-slate-900 mb-3 leading-tight">
                {course.title}
              </h1>
              {course.description && (
                <p className="text-slate-500 text-base leading-relaxed max-w-2xl">
                  {course.description}
                </p>
              )}
              <div className="flex items-center gap-1.5 mt-4 text-xs text-slate-400">
                <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" />
                </svg>
                {/* {course.creatorUsername ?? 'Instructor'} */}
              </div>
            </div>

            {/* Lessons scroll */}
            <div className="space-y-16">
              {course.sections.map((section, si) => (
                <div key={section.id}>
                  {/* Section divider */}
                  <div className="flex items-center gap-3 mb-8">
                    <span className="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-600 text-xs font-bold flex items-center justify-center flex-shrink-0">
                      {si + 1}
                    </span>
                    <h2 className="text-lg font-bold text-slate-800">{section.title}</h2>
                    <div className="flex-1 h-px bg-slate-200" />
                  </div>

                  {/* Lessons */}
                  <div className="space-y-12">
                    {section.lessons.map((lesson) => (
                      <LessonView
                        key={lesson.id}
                        lesson={lesson}
                        registerRef={registerRef}
                      />
                    ))}
                  </div>
                </div>
              ))}
            </div>

            {/* Footer */}
            <div className="mt-20 pt-8 border-t border-slate-200 text-center">
              <p className="text-sm text-slate-400">You've reached the end of the course</p>
              <p className="text-xs text-slate-300 mt-1">{course.title}</p>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   SIDEBAR SECTION
   ───────────────────────────────────────────────────────── */
function SidebarSection({
  section,
  sectionIndex,
  activeLessonId,
  onLessonClick,
}: {
  section: Section;
  sectionIndex: number;
  activeLessonId: string | null;
  onLessonClick: (id: string) => void;
}) {
  const [open, setOpen] = useState(true);

  return (
    <div>
      <button
        onClick={() => setOpen(!open)}
        className="w-full flex items-center gap-2.5 px-4 py-2.5 hover:bg-slate-800/60 transition-colors group"
      >
        <span className="w-5 h-5 rounded-md bg-slate-700 text-slate-400 text-xs font-bold flex items-center justify-center flex-shrink-0 group-hover:bg-indigo-900 group-hover:text-indigo-300 transition-colors">
          {sectionIndex + 1}
        </span>
        <span className="flex-1 text-left text-slate-300 text-xs font-semibold truncate">
          {section.title}
        </span>
        <svg
          width="12" height="12" viewBox="0 0 16 16" fill="none"
          stroke="currentColor" strokeWidth="2" strokeLinecap="round"
          className="text-slate-600 flex-shrink-0 transition-transform"
          style={{ transform: open ? 'rotate(180deg)' : 'rotate(0deg)' }}
        >
          <polyline points="4 6 8 10 12 6" />
        </svg>
      </button>

      {open && (
        <div className="pb-1">
          {section.lessons.map((lesson, li) => {
            const isActive = activeLessonId === lesson.id;
            const hasVideo = lesson.blocks.some((b) => b.type === 'Video');
            return (
              <button
                key={lesson.id}
                onClick={() => onLessonClick(lesson.id)}
                className={`w-full flex items-start gap-2.5 px-4 py-2 transition-colors text-left group
                  ${isActive
                    ? 'bg-indigo-600/20 border-r-2 border-indigo-400'
                    : 'hover:bg-slate-800/40 border-r-2 border-transparent'
                  }`}
              >
                <span className={`w-5 h-5 rounded flex items-center justify-center flex-shrink-0 mt-0.5 transition-colors
                  ${isActive ? 'text-indigo-400' : 'text-slate-600 group-hover:text-slate-400'}`}>
                  {hasVideo
                    ? <svg width="10" height="10" viewBox="0 0 24 24" fill="currentColor"><polygon points="5 3 19 12 5 21 5 3" /></svg>
                    : <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><line x1="4" y1="6" x2="20" y2="6" /><line x1="4" y1="12" x2="16" y2="12" /></svg>
                  }
                </span>
                <span className={`text-xs leading-relaxed line-clamp-2 transition-colors
                  ${isActive ? 'text-indigo-300 font-medium' : 'text-slate-400 group-hover:text-slate-300'}`}>
                  {li + 1}. {lesson.title}
                </span>
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   LESSON VIEW
   ───────────────────────────────────────────────────────── */
function LessonView({
  lesson,
  registerRef,
}: {
  lesson: Lesson;
  registerRef: (id: string, el: HTMLElement | null) => void;
}) {
  return (
    <section
      id={`lesson-${lesson.id}`}
      ref={(el) => registerRef(lesson.id, el)}
      className="scroll-mt-6"
    >
      <div className="flex items-center gap-2.5 mb-5">
        <span className="w-7 h-7 rounded-lg bg-slate-900 text-slate-300 flex items-center justify-center flex-shrink-0">
          <svg width="11" height="11" viewBox="0 0 24 24" fill="currentColor">
            <polygon points="5 3 19 12 5 21 5 3" />
          </svg>
        </span>
        <h3 className="text-xl font-bold text-slate-900">{lesson.title}</h3>
      </div>

      {lesson.blocks.length === 0 ? (
        <div className="rounded-xl bg-slate-100 py-10 text-center">
          <p className="text-sm text-slate-400">No content yet.</p>
        </div>
      ) : (
        <div className="space-y-6">
          {lesson.blocks.map((block) => (
            <BlockView key={block.id} block={block} />
          ))}
        </div>
      )}
    </section>
  );
}

/* ─────────────────────────────────────────────────────────
   BLOCK VIEW
   ───────────────────────────────────────────────────────── */
function BlockView({ block }: { block: Block }) {
  if (block.type === 'Video') return <VideoBlockView data={block.data as VideoBlockData} />;
  if (block.type === 'Text')  return <TextBlockView  data={block.data as TextBlockData} />;
  if (block.type === 'File')  return <FileBlockView  data={block.data as FileBlockData} />;
  return null;
}

/* ── Video Block ── */
function VideoBlockView({ data }: { data: VideoBlockData }) {
  const [playing, setPlaying] = useState(false);
  const url = data.sourceType === 'Url' ? data.url ?? '' : '';
  const embedUrl = getYouTubeEmbedUrl(url);
  const thumbnail = getYouTubeThumbnail(url);

  // Not a YouTube URL — show a plain video or a message
  if (!embedUrl && url) {
    return (
      <div className="rounded-2xl overflow-hidden bg-slate-900">
        <video controls className="w-full aspect-video" src={url}>
          Your browser does not support the video tag.
        </video>
      </div>
    );
  }

  if (!embedUrl) {
    return (
      <div className="rounded-2xl bg-slate-100 aspect-video flex items-center justify-center">
        <p className="text-sm text-slate-400">No video URL provided.</p>
      </div>
    );
  }

  // YouTube — show thumbnail until clicked (avoids autoplay issues + faster page load)
  if (!playing) {
    return (
      <div
        className="relative rounded-2xl overflow-hidden cursor-pointer group bg-slate-900 aspect-video"
        onClick={() => setPlaying(true)}
      >
        {thumbnail && (
          <img
            src={thumbnail}
            alt="Video thumbnail"
            className="w-full h-full object-cover opacity-80 group-hover:opacity-90 transition-opacity"
          />
        )}
        {/* Play button */}
        <div className="absolute inset-0 flex items-center justify-center">
          <div className="w-16 h-16 rounded-full bg-black/60 backdrop-blur-sm flex items-center justify-center group-hover:scale-110 group-hover:bg-indigo-600/80 transition-all duration-200 shadow-xl">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="white">
              <polygon points="5 3 19 12 5 21 5 3" />
            </svg>
          </div>
        </div>
        {/* YouTube badge */}
        <div className="absolute bottom-3 right-3 bg-black/70 backdrop-blur-sm rounded-lg px-2 py-1 flex items-center gap-1.5">
          <svg width="14" height="10" viewBox="0 0 90 63" fill="#FF0000">
            <path d="M88.2,9.9c-1-3.8-4-6.8-7.8-7.8C73.3,0,45,0,45,0S16.7,0,9.6,2.1C5.8,3.1,2.8,6.1,1.8,9.9C0,16.8,0,31.5,0,31.5S0,46.2,1.8,53.1c1,3.8,4,6.8,7.8,7.8C16.7,63,45,63,45,63s28.3,0,35.4-2.1c3.8-1,6.8-4,7.8-7.8C90,46.2,90,31.5,90,31.5S90,16.8,88.2,9.9z"/>
            <polygon points="36,45 59,31.5 36,18" fill="white"/>
          </svg>
          <span className="text-white text-xs font-medium">YouTube</span>
        </div>
      </div>
    );
  }

  // After click — embed the iframe
  return (
    <div className="rounded-2xl overflow-hidden bg-slate-900 aspect-video">
      <iframe
        src={`${embedUrl}&autoplay=1`}
        title="YouTube video"
        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
        allowFullScreen
        className="w-full h-full"
      />
    </div>
  );
}

/* ── Text Block ── */
function TextBlockView({ data }: { data: TextBlockData }) {
  if (!data.content) return null;
  return (
    <div className="prose prose-slate max-w-none">
      {data.content.split('\n').map((line, i) =>
        line.trim() === '' ? (
          <br key={i} />
        ) : (
          <p key={i} className="text-slate-700 leading-relaxed text-base my-0">
            {line}
          </p>
        )
      )}
    </div>
  );
}

/* ── File Block ── */
function FileBlockView({ data }: { data: FileBlockData }) {
  const name = data.fileName || 'Download file';
//   const url = data.fileUrl;

  return (
    <div className="flex items-center gap-4 p-4 bg-white rounded-xl border border-slate-200 hover:border-indigo-300 hover:bg-indigo-50/30 transition-colors group">
      <div className="w-10 h-10 rounded-lg bg-slate-100 group-hover:bg-indigo-100 flex items-center justify-center flex-shrink-0 transition-colors">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#6366f1" strokeWidth="1.5" strokeLinecap="round">
          <path d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z" />
          <polyline points="13 2 13 9 20 9" />
        </svg>
      </div>
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-slate-800 truncate">{name}</p>
        <p className="text-xs text-slate-400 mt-0.5">Attached file</p>
      </div>
      {/* {url ? (
        <a
          href={url}
          download
          className="flex items-center gap-1.5 px-3 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white text-xs font-medium rounded-lg transition-colors flex-shrink-0"
        >
          <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
            <polyline points="7 10 12 15 17 10" />
            <line x1="12" y1="15" x2="12" y2="3" />
          </svg>
          Download
        </a>
      ) : (
        <span className="text-xs text-slate-400 flex-shrink-0">No URL</span>
      )} */}
    </div>
  );
}

/* ─────────────────────────────────────────────────────────
   HELPERS
   ───────────────────────────────────────────────────────── */
function LevelBadge({ level }: { level: string }) {
  const map: Record<string, string> = {
    Beginner: 'bg-emerald-100 text-emerald-700',
    Intermediate: 'bg-amber-100 text-amber-700',
    Advanced: 'bg-red-100 text-red-700',
  };
  return (
    <span className={`px-2 py-0.5 rounded-full text-xs font-medium ${map[level] ?? 'bg-slate-100 text-slate-600'}`}>
      {level}
    </span>
  );
}

const font = { fontFamily: "'Outfit', sans-serif" };