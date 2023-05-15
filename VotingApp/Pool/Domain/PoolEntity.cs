using VotingApp.User.Domain;
using VotingApp.Option.Domain;
using VotingApp.Base.Domain;
namespace VotingApp.Pool.Domain;
public class PoolEntity : Entity
{


    public string Question { get; set; } = string.Empty;
    public Guid UserID { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual UserEntity User { get; set; } 
    public virtual IEnumerable<OptionEntity> Options { get; set; } 

}

