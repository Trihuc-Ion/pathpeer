using System;
using PathPeer.Domain.Enums;

namespace PathPeer.Application.Features.Courses.DTOs;

public class VoteDto
{
    public VoteType Type { get; set; } // Up sau Down
}

public class VoteResultDto
{
    public int VotesUp { get; set; }
    public int VotesDown { get; set; }
    public VoteType? UserVote { get; set; } // votul userului curent
}
