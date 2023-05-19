using VotingApp.Poll.Domain;
using VotingApp.Vote.Domain;
using VotingApp.Base.Domain;
namespace VotingApp.Option.Domain;

public class OptionEntity : Entity
{
 
    public Guid PollID { get; set; }
    public string Text { get; set; } = String.Empty;
    public virtual PollEntity Poll { get; set; } 
    public virtual IEnumerable<VoteEntity> Votes { get; set; }

}

