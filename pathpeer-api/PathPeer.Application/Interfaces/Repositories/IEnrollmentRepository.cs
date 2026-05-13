using System;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetEnrollmentAsync(int userId, int courseId);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(int userId);
    Task<bool> ExistsEnrollmentAsync(int userId, int courseId);
    Task CreateEnrollmentAsync(Enrollment enrollment);
    Task DeleteEnrollmentAsync(Enrollment enrollment);
}
