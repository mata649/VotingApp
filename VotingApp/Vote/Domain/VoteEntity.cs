using VotingApp.User.Domain;
using VotingApp.Option.Domain;
using VotingApp.Base.Domain;
namespace VotingApp.Vote.Domain;

public class VoteEntity : Entity
{

    public Guid UserID { get; set; }
    public Guid OptionID { get; set; }
    public virtual UserEntity User { get; set; } 
    public virtual OptionEntity Option { get; set; }
}

