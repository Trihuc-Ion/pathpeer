using System;
using System.ComponentModel.DataAnnotations;

namespace PathPeer.Application.Features.Auth.DTOs;

public class RegisterDto
{
    [Required]
    [MaxLength(25)]
    public string UserName { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    public string Password { get; set; } = null!;
}
