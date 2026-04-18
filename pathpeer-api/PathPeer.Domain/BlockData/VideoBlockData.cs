using System;
using PathPeer.Domain.Enums;

namespace PathPeer.Domain.BlockData;

public class VideoBlockData
{
    public VideoSourceType SourceType { get; set; } // Url sau File

    public string? Url { get; set; }
    public string? FileName { get; set; }
}
