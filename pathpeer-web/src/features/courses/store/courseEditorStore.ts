import { create } from 'zustand';
import type {
  Course,
  Section,
  Lesson,
  Block,
  BlockType,
  VideoBlockData,
  TextBlockData,
  FileBlockData,
  CourseLevel,
} from '../../../shared/types/course';

const genId = (): string => `local-${Date.now()}-${Math.random().toString(36).slice(2, 7)}`;

/** Returns true when the id was generated locally (not yet persisted to the server) */
export const isLocalId = (id: string): boolean => id.startsWith('local-');

const emptyCourse = (): Course => ({
  id: genId(),
  title: '',
  description: '',
  language: 'English',
  price: 0,
  level: 'Beginner' as CourseLevel,
  status: 'Draft',
  version: 1,
  creatorId: 0,
  sections: [],
});

interface CourseState {
  course: Course;
  /** IDs created in this session (need POST to persist) */
  localIds: Set<string>;

  initCreate: () => void;
  /** Called by CoursePage after fetching from API */
  setCourse: (course: Course) => void;
  updateCourseField: <K extends keyof Course>(key: K, value: Course[K]) => void;

  addSection: () => void;
  updateSectionTitle: (sectionId: string, title: string) => void;
  toggleSection: (sectionId: string) => void;
  deleteSection: (sectionId: string) => void;
  reorderSections: (ids: string[]) => void;

  addLesson: (sectionId: string) => void;
  updateLessonTitle: (sectionId: string, lessonId: string, title: string) => void;
  toggleLesson: (sectionId: string, lessonId: string) => void;
  deleteLesson: (sectionId: string, lessonId: string) => void;
  reorderLessons: (sectionId: string, ids: string[]) => void;

  addBlock: (sectionId: string, lessonId: string, type: BlockType) => void;
  updateBlock: (sectionId: string, lessonId: string, blockId: string, data: Partial<Block['data']>) => void;
  deleteBlock: (sectionId: string, lessonId: string, blockId: string) => void;
  reorderBlocks: (sectionId: string, lessonId: string, ids: string[]) => void;
}

export const useCourseStore = create<CourseState>((set) => ({
  course: emptyCourse(),
  localIds: new Set<string>(),

  initCreate: () => {
    const course = emptyCourse();
    set({ course, localIds: new Set([course.id]) });
  },

  setCourse: (course) =>
    set({ course, localIds: new Set<string>() }),

  updateCourseField: (key, value) =>
    set((state) => ({ course: { ...state.course, [key]: value } })),

  addSection: () =>
    set((state) => {
      const id = genId();
      const newSection: Section = {
        id, title: 'New Section', order: state.course.sections.length, isExpanded: true, lessons: [],
      };
      return {
        course: { ...state.course, sections: [...state.course.sections, newSection] },
        localIds: new Set([...state.localIds, id]),
      };
    }),

  updateSectionTitle: (sectionId, title) =>
    set((state) => ({
      course: {
        ...state.course,
        sections: state.course.sections.map((s) => s.id === sectionId ? { ...s, title } : s),
      },
    })),

  toggleSection: (sectionId) =>
    set((state) => ({
      course: {
        ...state.course,
        sections: state.course.sections.map((s) =>
          s.id === sectionId ? { ...s, isExpanded: !s.isExpanded } : s
        ),
      },
    })),

  deleteSection: (sectionId) =>
    set((state) => {
      const newLocalIds = new Set(state.localIds);
      newLocalIds.delete(sectionId);
      return {
        course: {
          ...state.course,
          sections: state.course.sections.filter((s) => s.id !== sectionId).map((s, i) => ({ ...s, order: i })),
        },
        localIds: newLocalIds,
      };
    }),

  reorderSections: (ids) =>
    set((state) => {
      const map = new Map(state.course.sections.map((s) => [s.id, s]));
      return { course: { ...state.course, sections: ids.map((id, i) => ({ ...map.get(id)!, order: i })) } };
    }),

  addLesson: (sectionId) =>
    set((state) => {
      const id = genId();
      return {
        course: {
          ...state.course,
          sections: state.course.sections.map((s) => {
            if (s.id !== sectionId) return s;
            const newLesson: Lesson = { id, title: 'New Lesson', order: s.lessons.length, isExpanded: true, blocks: [] };
            return { ...s, lessons: [...s.lessons, newLesson] };
          }),
        },
        localIds: new Set([...state.localIds, id]),
      };
    }),

  updateLessonTitle: (sectionId, lessonId, title) =>
    set((state) => ({
      course: {
        ...state.course,
        sections: state.course.sections.map((s) =>
          s.id !== sectionId ? s : { ...s, lessons: s.lessons.map((l) => (l.id === lessonId ? { ...l, title } : l)) }
        ),
      },
    })),

  toggleLesson: (sectionId, lessonId) =>
    set((state) => ({
      course: {
        ...state.course,
        sections: state.course.sections.map((s) =>
          s.id !== sectionId ? s : {
            ...s,
            lessons: s.lessons.map((l) => l.id === lessonId ? { ...l, isExpanded: !l.isExpanded } : l),
          }
        ),
      },
    })),

  deleteLesson: (sectionId, lessonId) =>
    set((state) => {
      const newLocalIds = new Set(state.localIds);
      newLocalIds.delete(lessonId);
      return {
        course: {
          ...state.course,
          sections: state.course.sections.map((s) =>
            s.id !== sectionId ? s : {
              ...s,
              lessons: s.lessons.filter((l) => l.id !== lessonId).map((l, i) => ({ ...l, order: i })),
            }
          ),
        },
        localIds: newLocalIds,
      };
    }),

  reorderLessons: (sectionId, ids) =>
    set((state) => ({
      course: {
        ...state.course,
        sections: state.course.sections.map((s) => {
          if (s.id !== sectionId) return s;
          const map = new Map(s.lessons.map((l) => [l.id, l]));
          return { ...s, lessons: ids.map((id, i) => ({ ...map.get(id)!, order: i })) };
        }),
      },
    })),

  addBlock: (sectionId, lessonId, type) =>
    set((state) => {
      const id = genId();
      const defaultData = (): Block['data'] => {
        if (type === 'Video') return { sourceType: 'Url', url: '' } as VideoBlockData;
        if (type === 'Text') return { content: '' } as TextBlockData;
        return { fileName: '' } as FileBlockData;
      };
      return {
        course: {
          ...state.course,
          sections: state.course.sections.map((s) =>
            s.id !== sectionId ? s : {
              ...s,
              lessons: s.lessons.map((l) => {
                if (l.id !== lessonId) return l;
                const newBlock: Block = { id, type, order: l.blocks.length, data: defaultData() };
                return { ...l, blocks: [...l.blocks, newBlock] };
              }),
            }
          ),
        },
        localIds: new Set([...state.localIds, id]),
      };
    }),

  updateBlock: (sectionId, lessonId, blockId, data) =>
    set((state) => ({
      course: {
        ...state.course,
        sections: state.course.sections.map((s) =>
          s.id !== sectionId ? s : {
            ...s,
            lessons: s.lessons.map((l) =>
              l.id !== lessonId ? l : {
                ...l,
                blocks: l.blocks.map((b) => b.id !== blockId ? b : { ...b, data: { ...b.data, ...data } }),
              }
            ),
          }
        ),
      },
    })),

  deleteBlock: (sectionId, lessonId, blockId) =>
    set((state) => {
      const newLocalIds = new Set(state.localIds);
      newLocalIds.delete(blockId);
      return {
        course: {
          ...state.course,
          sections: state.course.sections.map((s) =>
            s.id !== sectionId ? s : {
              ...s,
              lessons: s.lessons.map((l) =>
                l.id !== lessonId ? l : {
                  ...l,
                  blocks: l.blocks.filter((b) => b.id !== blockId).map((b, i) => ({ ...b, order: i })),
                }
              ),
            }
          ),
        },
        localIds: newLocalIds,
      };
    }),

  reorderBlocks: (sectionId, lessonId, ids) =>
    set((state) => ({
      course: {
        ...state.course,
        sections: state.course.sections.map((s) =>
          s.id !== sectionId ? s : {
            ...s,
            lessons: s.lessons.map((l) => {
              if (l.id !== lessonId) return l;
              const map = new Map(l.blocks.map((b) => [b.id, b]));
              return { ...l, blocks: ids.map((id, i) => ({ ...map.get(id)!, order: i })) };
            }),
          }
        ),
      },
    })),
}));