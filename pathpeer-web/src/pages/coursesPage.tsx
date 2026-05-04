import React, { useState, useMemo } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { courseApi } from '../features/courses/api/courseApi';
import { useAuthStore } from '../features/auth/store/authStore'; // adjust path
import type { Course, CourseLevel } from '../shared/types/course';

type Tab = 'all' | 'mine';

const LEVELS: CourseLevel[] = ['Beginner', 'Intermediate', 'Advanced'];

const LEVEL_META: Record<string, { color: string; bg: string; dot: string }> = {
    Beginner: { color: 'text-emerald-700', bg: 'bg-emerald-50', dot: 'bg-emerald-400' },
    Intermediate: { color: 'text-amber-700', bg: 'bg-amber-50', dot: 'bg-amber-400' },
    Advanced: { color: 'text-red-700', bg: 'bg-red-50', dot: 'bg-red-400' },
};

// Generates a deterministic gradient from course title
const CARD_GRADIENTS = [
    'from-indigo-500 to-violet-600',
    'from-sky-500 to-indigo-600',
    'from-emerald-500 to-teal-600',
    'from-amber-500 to-orange-600',
    'from-rose-500 to-pink-600',
    'from-teal-500 to-cyan-600',
];
const getGradient = (title: string) =>
    CARD_GRADIENTS[title.charCodeAt(0) % CARD_GRADIENTS.length];

export default function CoursesPage() {
    const navigate = useNavigate();
    const currentUser = useAuthStore((s) => s.user);
    const [tab, setTab] = useState<Tab>('all');
    const [search, setSearch] = useState('');
    const [levelFilter, setLevelFilter] = useState<CourseLevel | 'All'>('All');

    const { data: courses = [], isLoading, error } = useQuery({
        queryKey: ['courses'],
        queryFn: courseApi.getCourses,
    });

    /* ── Filter logic ── */
    const filtered = useMemo(() => {
        let list = courses;

        if (tab === 'mine') {
            list = list.filter((c) => c.creatorId === currentUser?.id);
        }

        if (levelFilter !== 'All') {
            list = list.filter((c) => c.level === levelFilter);
        }

        if (search.trim()) {
            const q = search.toLowerCase();
            list = list.filter(
                (c) =>
                    c.title.toLowerCase().includes(q) ||
                    c.description?.toLowerCase().includes(q),
                //   c.creatorUsername?.toLowerCase().includes(q)
            );
        }

        return list;
    }, [courses, tab, levelFilter, search, currentUser?.id]);

    const myCourseCount = courses.filter((c) => c.creatorId === currentUser?.id).length;

    return (
        <div className="min-h-screen bg-stone-50" style={{ fontFamily: "'Outfit', sans-serif" }}>

            {/* ── Header ── */}
            <header className="bg-white border-b border-stone-200 sticky top-0 z-40">
                <div className="max-w-5xl mx-auto px-5 sm:px-8 h-14 flex items-center justify-between gap-4">
                    <Link to="/" className="flex items-center gap-3">
                        <div className="w-7 h-7 rounded-lg bg-indigo-600 flex items-center justify-center shadow-sm">
                            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.5">
                                <path d="M12 2L2 7l10 5 10-5-10-5z" />
                                <path d="M2 17l10 5 10-5" />
                                <path d="M2 12l10 5 10-5" />
                            </svg>
                        </div>
                        <span className="font-semibold text-stone-800 text-sm">PathPeer</span>
                    </Link>
                    <Link
                        to="/courses/create"
                        className="flex items-center gap-1.5 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-sm font-semibold transition-colors shadow-sm shadow-indigo-200"
                    >
                        <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                            <line x1="12" y1="5" x2="12" y2="19" />
                            <line x1="5" y1="12" x2="19" y2="12" />
                        </svg>
                        New Course
                    </Link>
                </div>
            </header>

            <main className="max-w-5xl mx-auto px-5 sm:px-8 py-8">

                {/* ── Page title ── */}
                <div className="mb-6">
                    <h1 className="text-2xl font-bold text-stone-900 tracking-tight">Courses</h1>
                    <p className="text-sm text-stone-500 mt-0.5">
                        {isLoading ? 'Loading…' : `${courses.length} course${courses.length !== 1 ? 's' : ''} available`}
                    </p>
                </div>

                {/* ── Tabs + Search + Filter row ── */}
                <div className="flex flex-col sm:flex-row sm:items-center gap-3 mb-6">

                    {/* Tabs */}
                    <div className="flex items-center bg-stone-100 rounded-xl p-1 flex-shrink-0">
                        <TabButton active={tab === 'all'} onClick={() => setTab('all')}>
                            All Courses
                            <span className="ml-1.5 px-1.5 py-0.5 bg-stone-200 rounded-md text-xs font-semibold text-stone-600">
                                {courses.length}
                            </span>
                        </TabButton>
                        <TabButton active={tab === 'mine'} onClick={() => setTab('mine')}>
                            My Courses
                            {myCourseCount > 0 && (
                                <span className="ml-1.5 px-1.5 py-0.5 bg-indigo-100 rounded-md text-xs font-semibold text-indigo-600">
                                    {myCourseCount}
                                </span>
                            )}
                        </TabButton>
                    </div>

                    <div className="flex items-center gap-2 flex-1">
                        {/* Search */}
                        <div className="relative flex-1">
                            <svg
                                width="14" height="14" viewBox="0 0 24 24" fill="none"
                                stroke="currentColor" strokeWidth="2"
                                className="absolute left-3 top-1/2 -translate-y-1/2 text-stone-400 pointer-events-none"
                            >
                                <circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" />
                            </svg>
                            <input
                                type="text"
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                                placeholder="Search courses…"
                                className="w-full pl-9 pr-4 py-2 rounded-xl border border-stone-200 text-sm text-stone-900 placeholder-stone-400 focus:outline-none focus:ring-2 focus:ring-indigo-500 bg-white"
                            />
                            {search && (
                                <button
                                    onClick={() => setSearch('')}
                                    className="absolute right-3 top-1/2 -translate-y-1/2 text-stone-400 hover:text-stone-600"
                                >
                                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                                        <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                                    </svg>
                                </button>
                            )}
                        </div>

                        {/* Level filter */}
                        <div className="relative flex-shrink-0">
                            <select
                                value={levelFilter}
                                onChange={(e) => setLevelFilter(e.target.value as CourseLevel | 'All')}
                                className="appearance-none pl-3 pr-8 py-2 rounded-xl border border-stone-200 text-sm text-stone-700 bg-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                            >
                                <option value="All">All levels</option>
                                {LEVELS.map((l) => <option key={l} value={l}>{l}</option>)}
                            </select>
                            <div className="pointer-events-none absolute right-2.5 top-1/2 -translate-y-1/2 text-stone-400">
                                <svg width="12" height="12" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round">
                                    <polyline points="4 6 8 10 12 6" />
                                </svg>
                            </div>
                        </div>
                    </div>
                </div>

                {/* ── Error ── */}
                {error && (
                    <div className="bg-red-50 border border-red-200 rounded-xl px-4 py-3 text-sm text-red-600 mb-6">
                        Failed to load courses. Please try again.
                    </div>
                )}

                {/* ── Loading skeleton ── */}
                {isLoading && (
                    <div className="space-y-3">
                        {[1, 2, 3, 4].map((i) => (
                            <div key={i} className="bg-white rounded-2xl border border-stone-100 h-28 animate-pulse" />
                        ))}
                    </div>
                )}

                {/* ── Empty state ── */}
                {!isLoading && filtered.length === 0 && (
                    <div className="bg-white rounded-2xl border border-dashed border-stone-300 py-16 text-center">
                        <div className="w-12 h-12 rounded-xl bg-stone-100 flex items-center justify-center mx-auto mb-3">
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#a8a29e" strokeWidth="1.5">
                                <circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" />
                            </svg>
                        </div>
                        <h3 className="font-semibold text-stone-700 mb-1">
                            {tab === 'mine' ? "You haven't created any courses yet" : 'No courses found'}
                        </h3>
                        <p className="text-sm text-stone-400 mb-5">
                            {search || levelFilter !== 'All'
                                ? 'Try adjusting your search or filters.'
                                : tab === 'mine'
                                    ? 'Create your first course to get started.'
                                    : 'No courses available yet.'}
                        </p>
                        {tab === 'mine' && (
                            <Link
                                to="/courses/create"
                                className="px-5 py-2.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-xl text-sm font-semibold transition-colors"
                            >
                                + Create First Course
                            </Link>
                        )}
                    </div>
                )}

                {/* ── Course list ── */}
                {!isLoading && filtered.length > 0 && (
                    <div className="space-y-3">
                        {filtered.map((course) => (
                            <CourseRow
                                key={course.id}
                                course={course}
                                isOwner={course.creatorId === currentUser?.id}
                            />
                        ))}
                    </div>
                )}
            </main>
        </div>
    );
}

/* ─────────────────────────────────────────────────────────
   COURSE ROW
   ───────────────────────────────────────────────────────── */
function CourseRow({ course, isOwner }: { course: Course; isOwner: boolean }) {
    const level = LEVEL_META[course.level] ?? LEVEL_META['Beginner'];
    const gradient = getGradient(course.title);
    const totalLessons = course.sections?.reduce((sum, s) => sum + s.lessons.length, 0) ?? 0;
    const totalSections = course.sections?.length ?? 0;

    return (
        <div className="bg-white rounded-2xl border border-stone-100 hover:border-stone-200 hover:shadow-md transition-all duration-200 overflow-hidden flex group">

            {/* Thumbnail */}
            <Link to={`/courses/${course.id}`} className="flex-shrink-0">
                <div className={`w-40 sm:w-48 h-full min-h-[7rem] bg-gradient-to-br ${gradient} flex items-center justify-center`}>
                    <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="1.5" opacity="0.7">
                        <path d="M12 2L2 7l10 5 10-5-10-5z" />
                        <path d="M2 17l10 5 10-5" />
                        <path d="M2 12l10 5 10-5" />
                    </svg>
                </div>
            </Link>

            {/* Content */}
            <div className="flex-1 min-w-0 px-5 py-4 flex flex-col justify-between">
                <div>
                    <div className="flex items-start justify-between gap-3 mb-1.5">
                        <Link to={`/courses/${course.id}`} className="group/title">
                            <h3 className="font-semibold text-stone-900 text-base leading-snug group-hover/title:text-indigo-600 transition-colors line-clamp-2">
                                {course.title}
                            </h3>
                        </Link>

                        {/* Price */}
                        <span className="flex-shrink-0 text-base font-bold text-stone-900">
                            {course.price === 0 ? (
                                <span className="text-emerald-600 text-sm font-semibold">Free</span>
                            ) : (
                                `$${course.price}`
                            )}
                        </span>
                    </div>

                    {course.description && (
                        <p className="text-xs text-stone-500 line-clamp-2 leading-relaxed mb-2">
                            {course.description}
                        </p>
                    )}
                </div>

                {/* Meta row */}
                <div className="flex items-center justify-between gap-3 flex-wrap">
                    <div className="flex items-center gap-3 flex-wrap">
                        {/* Level */}
                        <span className={`flex items-center gap-1.5 px-2 py-0.5 rounded-full text-xs font-medium ${level.bg} ${level.color}`}>
                            <span className={`w-1.5 h-1.5 rounded-full ${level.dot}`} />
                            {course.level}
                        </span>

                        {/* Language */}
                        <span className="text-xs text-stone-400">{course.language}</span>

                        {/* Stats */}
                        {totalSections > 0 && (
                            <span className="text-xs text-stone-400 hidden sm:inline">
                                {totalSections} section{totalSections !== 1 ? 's' : ''}
                                {totalLessons > 0 && ` · ${totalLessons} lesson${totalLessons !== 1 ? 's' : ''}`}
                            </span>
                        )}

                        {/* Instructor */}
                        <span className="text-xs text-stone-400 flex items-center gap-1">
                            <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" /><circle cx="12" cy="7" r="4" />
                            </svg>
                            {/* {course.creatorUsername} */}
                        </span>

                        {/* Status badge */}
                        {/* {isOwner && (
              <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${
                course.status === 'Published'
                  ? 'bg-emerald-50 text-emerald-600'
                  : course.status === 'Review'
                  ? 'bg-blue-50 text-blue-600'
                  : 'bg-amber-50 text-amber-600'
              }`}>
                {course.status}
              </span>
            )} */}
                    </div>

                    {/* Actions */}
                    <div className="flex items-center gap-2">
                        {isOwner && (
                            <Link
                                to={`/courses/${course.id}/edit`}
                                className="flex items-center gap-1.5 px-3 py-1.5 border border-stone-200 hover:border-indigo-300 hover:text-indigo-600 hover:bg-indigo-50 rounded-lg text-xs font-medium text-stone-600 transition-colors"
                            >
                                <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round">
                                    <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                                    <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                                </svg>
                                Edit
                            </Link>
                        )}
                        <Link
                            to={`/courses/${course.id}`}
                            className="flex items-center gap-1.5 px-3 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-xs font-semibold transition-colors"
                        >
                            <svg width="11" height="11" viewBox="0 0 24 24" fill="currentColor">
                                <polygon points="5 3 19 12 5 21 5 3" />
                            </svg>
                            Open
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    );
}

/* ─────────────────────────────────────────────────────────
   TAB BUTTON
   ───────────────────────────────────────────────────────── */
function TabButton({
    active,
    onClick,
    children,
}: {
    active: boolean;
    onClick: () => void;
    children: React.ReactNode;
}) {
    return (
        <button
            onClick={onClick}
            className={`flex items-center px-3 py-1.5 rounded-lg text-sm font-medium transition-colors ${active
                    ? 'bg-white text-stone-900 shadow-sm'
                    : 'text-stone-500 hover:text-stone-700'
                }`}
        >
            {children}
        </button>
    );
}