using Microsoft.Extensions.Logging;
using PathPeer.Application.Features.Users.DTOs;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IEmailService emailService, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Userul nu există.");

        user.Username = dto.Username ?? user.Username;
        user.Bio = dto.Bio ?? user.Bio;
        user.ProfilePictureUrl = dto.ProfilePictureUrl ?? user.ProfilePictureUrl;
        user.TeacherBio = dto.TeacherBio ?? user.TeacherBio;

        await _userRepository.UpdateUserAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            TeacherStatus = user.TeacherStatus.ToString(),
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            TeacherBio = user.TeacherBio,
            TeacherApprovedAt = user.TeacherApprovedAt
        };
    }

    public async Task RequestTeacherAsync(int userId, RequestTeacherDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("Userul nu există.");

        if (user.TeacherStatus != TeacherStatus.NotRequested && user.TeacherStatus != TeacherStatus.Rejected)
            throw new InvalidOperationException("Ai deja o cerere în așteptare sau ești deja profesor.");

        user.TeacherStatus = TeacherStatus.Pending;
        if (dto.TeacherBio != null) user.TeacherBio = dto.TeacherBio;

        await _userRepository.UpdateUserAsync(user);
    }

    public async Task ApproveTeacherAsync(int targetUserId)
    {
        var user = await _userRepository.GetByIdAsync(targetUserId)
            ?? throw new KeyNotFoundException("Userul nu există.");

        if (user.TeacherStatus != TeacherStatus.Pending)
            throw new InvalidOperationException("Userul nu are o cerere în așteptare.");

        user.Role = UserRole.Teacher;
        user.TeacherStatus = TeacherStatus.Approved;
        user.TeacherApprovedAt = DateTime.UtcNow;

        await _userRepository.UpdateUserAsync(user);
        _logger.LogInformation("Teacher aprobat: userId={UserId}, username={Username}", targetUserId, user.Username);
        await _emailService.SendTeacherApprovedAsync(user.Email, user.Username);
    }

    public async Task RejectTeacherAsync(int targetUserId)
    {
        var user = await _userRepository.GetByIdAsync(targetUserId)
            ?? throw new KeyNotFoundException("Userul nu există.");

        if (user.TeacherStatus != TeacherStatus.Pending)
            throw new InvalidOperationException("Userul nu are o cerere în așteptare.");

        user.TeacherStatus = TeacherStatus.Rejected;

        await _userRepository.UpdateUserAsync(user);
        _logger.LogInformation("Teacher respins: userId={UserId}, username={Username}", targetUserId, user.Username);
        await _emailService.SendTeacherRejectedAsync(user.Email, user.Username);
    }
}