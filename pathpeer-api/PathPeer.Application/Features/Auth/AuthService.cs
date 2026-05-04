using System;
using Microsoft.Extensions.Options;
using PathPeer.Application.Common;
using PathPeer.Application.Features.Auth.DTOs;
using PathPeer.Application.Features.Users.DTOs;
using PathPeer.Application.Interfaces.Repositories;
using PathPeer.Application.Interfaces.Services;
using PathPeer.Domain.Entities;

namespace PathPeer.Application.Features.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        
        if (existingUser != null)
            throw new Exception("Email-ul există deja");

        var user = new User
        {
            Email = dto.Email,
            Username = dto.UserName,
            PasswordHash = _passwordHasher.Hash(dto.Password),
        };

        var createdUser = await _userRepository.CreateAsync(user);

        // 4. Generezi JWT token
        var token = _tokenService.GenerateToken(createdUser);

        // 5. Returnezi AuthResponseDto
        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            User = new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Role = createdUser.Role.ToString(),
                TeacherStatus = createdUser.TeacherStatus.ToString()
            }
        };
        
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new Exception("Email sau parolă incorectă");

        if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Email sau parolă incorectă");
        
        var token = _tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(24),
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                TeacherStatus = user.TeacherStatus.ToString()
            }
        };
    }

    public async Task<UserDto> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Utilizatorul nu a fost găsit");

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            TeacherStatus = user.TeacherStatus.ToString()
        };
    }
}