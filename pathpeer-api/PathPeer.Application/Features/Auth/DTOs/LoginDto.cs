using System;
using System.ComponentModel.DataAnnotations;

namespace PathPeer.Application.Features.Auth.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
