using VotingApp.Vote.Domain;
using VotingApp.Pool.Domain;
using VotingApp.Base.Domain;

namespace VotingApp.User.Domain;
public class UserEntity : Entity
{   
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public virtual IEnumerable<PoolEntity> Pools { get; set; } 
    public virtual IEnumerable<VoteEntity> Votes { get; set; } 

}

