using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ILessonBlockRepository
{
    Task AddLessonBlockAsync(LessonBlock block);
    Task<int> GetLastLessonBlockOrderAsync(int lessonId);
    Task<List<LessonBlock>> GetLessonBlocksByLessonIdAsync(int lessonId);
    Task<LessonBlock?> GetLessonBlockByIdAsync(int blockId);
    Task UpdateLessonBlockAsync(LessonBlock block);
    Task DeleteLessonBlockAsync(LessonBlock block);
    Task ReorderLessonBlocksAsync(int lessonId, List<int> orderedLessonBlockIds);
}
