using System.Text.Json;
using System.Text.Json.Serialization;
using PathPeer.Domain.BlockData;
using PathPeer.Domain.Entities;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.Helpers;

public static class BlockDataHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() } 
    };

    public static object Deserialize(LessonBlock block)
    {
        return block.Type switch
        {
            BlockType.Video => JsonSerializer.Deserialize<VideoBlockData>(block.Data, Options)!,
            BlockType.Text => JsonSerializer.Deserialize<TextBlockData>(block.Data, Options)!,
            BlockType.File => JsonSerializer.Deserialize<FileBlockData>(block.Data, Options)!,
            _ => throw new Exception("Unknown block type")
        };
    }
}
