using VotingApp.Pool.Domain;
using VotingApp.Vote.Domain;
using VotingApp.Base.Domain;
namespace VotingApp.Option.Domain;

public class OptionEntity : Entity
{
 
    public Guid PoolID { get; set; }
    public string Text { get; set; } = String.Empty;
    public virtual PoolEntity Pool { get; set; } 
    public virtual IEnumerable<VoteEntity> Votes { get; set; }

}

