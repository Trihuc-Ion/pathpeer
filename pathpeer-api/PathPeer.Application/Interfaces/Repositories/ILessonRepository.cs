using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ILessonRepository
{
    Task AddLessonAsync(Lesson lesson);
    Task<int> GetLastLessonOrderAsync(int sectionId);
    Task<Lesson?> GetLessonByIdAsync(int lessonId);
    Task UpdateLessonAsync(Lesson lesson);
    Task DeleteLessonAsync(Lesson lesson);
    Task ReorderLessonsAsync(int sectionId, List<int> orderedLessonIds);
}
