using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.DTOs;

public class CreateLessonBlockDto
{
    [Required]
    public BlockType Type { get; set; }
    [Required]
    public JsonElement Data { get; set; }

    public bool IsDataValid() => 
        Data.ValueKind != JsonValueKind.Null && 
        Data.ValueKind != JsonValueKind.Undefined;
}