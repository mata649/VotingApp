using VotingApp.Base.Domain;

namespace VotingApp.Poll.Domain.DTO;

public class UpdatePollDTO : UpdateDTO<PollEntity>
{
    public string Question { get; set; } = string.Empty;

    public override PollEntity ToEntity() => new() { ID = ID, Question = Question, };


}

