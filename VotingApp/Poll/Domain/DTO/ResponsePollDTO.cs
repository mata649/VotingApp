using VotingApp.Base.Domain; 

namespace VotingApp.Poll.Domain.DTO;

public class ResponsePollDTO : Entity
{
    public string Question { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid UserID { get; set; }
    public static ResponsePollDTO FromPoll(PollEntity poll) => new()
    {
        ID = poll.ID,
        Question = poll.Question,
        CreatedAt = poll.CreatedAt,
        UserID = poll.UserID

    };
}

