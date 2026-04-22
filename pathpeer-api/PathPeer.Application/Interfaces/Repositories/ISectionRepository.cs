using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface ISectionRepository
{
    Task AddSectionAsync(Section section);
    Task<int> GetLastSectionOrderAsync(int courseId);
    
}
