using VotingApp.Base.Domain;
using VotingApp.Option.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Domain
{
    public interface IVoteNotificationService
    {
        
        public IResponse GetCountOfVotes(Guid pollID);
        public IResponse AddVoteToDashboard(OptionEntity option);
    }
}
