using VotingApp.User.Domain;
using VotingApp.Option.Domain;
using VotingApp.Base.Domain;
namespace VotingApp.Poll.Domain;
public class PollEntity : Entity
{


    public string Question { get; set; } = string.Empty;
    public Guid UserID { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual UserEntity User { get; set; } 
    public virtual IEnumerable<OptionEntity> Options { get; set; } 

}

