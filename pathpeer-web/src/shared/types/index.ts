export const UserRole = {
    User: 0,
    Admin: 1
} as const;
export type UserRole = typeof UserRole[keyof typeof UserRole];

export const TeacherStatus = {
    NotRequested: 0,
    Pending: 1,
    Approved: 2,
    Rejected: 3
} as const;
export type TeacherStatus = typeof TeacherStatus[keyof typeof TeacherStatus];

export const CourseLevel = {
    Beginner: 0,
    Intermediate: 1,
    Advanced: 2
} as const;
export type CourseLevel = typeof CourseLevel[keyof typeof CourseLevel];


export const CourseStatus = {
    Draft: 0,
    InReviewCloud: 1,
    ApprovedInCloud: 2,
    Rejected: 3
} as const;
export type CourseStatus = typeof CourseStatus[keyof typeof CourseStatus];

export interface User {
    id: number;
    email: string;
    username: string;
    profilePictureUrl?: string;
    bio?: string;
    role: UserRole;
    teacherStatus: TeacherStatus;
    teacherBio?: string;
    teacherApprovedAt: string | null;
    createdAt: string;
    // courses: Course[];
}

export interface Course {
    id: number;
    title: string;
    description?: string;
    category?: string;
    price: number;
    language?: string;
    level?: CourseLevel;
    status: CourseStatus;
    version: number;
    reviewStartedAt?: string;
    reviewEndsAt?: string;
    votesUp: number;
    votesDown: number;
    createdAt: string;
    updatedAt: string | null;
    creatorId: number;
    creatorUsername: string;
}