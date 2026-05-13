using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ISectionRepository
{
    Task AddSectionAsync(Section section);
    Task<int> GetLastSectionOrderAsync(int courseId);
    Task<Section?> GetSectionByIdAsync(int sectionId);
    Task UpdateSectionAsync(Section section);
    Task DeleteSectionAsync(Section section);
    Task ReorderSectionsAsync(int courseId, List<int> orderedSectionIds);
}
