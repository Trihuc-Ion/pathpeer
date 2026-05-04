export type BlockType = 'Video' | 'Text' | 'File';
export type VideoSourceType = 'Url' | 'File';
export type CourseLevel = 'Beginner' | 'Intermediate' | 'Advanced';
export type CourseStatus = 'Draft' | 'InReviewCloud' | 'ApprovedInCloud' | 'Rejected';

export interface VideoBlockData {
  sourceType: VideoSourceType;
  url?: string;
  fileName?: string;
}

export interface TextBlockData {
  content: string;
}

export interface FileBlockData {
//   fileUrl?: string;
  fileName?: string;
}

export type BlockData = VideoBlockData | TextBlockData | FileBlockData;

export interface Block {
  id: string;
  type: BlockType;
  order: number;
  data: BlockData;
}

export interface Lesson {
  id: string;
  title: string;
  order: number;
  isExpanded: boolean;
  blocks: Block[];
}

export interface Section {
  id: string;
  title: string;
  order: number;
  isExpanded: boolean;
  lessons: Lesson[];
}

export interface Course {
  id: string;
  title: string;
  description: string;
  language: string;
  price: number;
  level: CourseLevel;
  status: CourseStatus;
  version: number;
//     createdAt: string;
//     updatedAt: string | null;
    creatorId: number;
//     creatorUsername: string;
  sections: Section[];
}