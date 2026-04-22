using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ILessonRepository
{
    Task AddLessonAsync(Lesson lesson);
    Task<int> GetLastLessonOrderAsync(int sectionId);

}
