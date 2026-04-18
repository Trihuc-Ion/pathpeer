using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ILessonBlockRepository
{
    Task AddLessonBlockAsync(LessonBlock block);
    Task<int> GetLastLessonBlockOrderAsync(int lessonId);
    Task<List<LessonBlock>> GetLessonBlocksByLessonIdAsync(int lessonId);
}
