using VotingApp.Base.Domain;
using VotingApp.Option.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Domain
{
    public interface IVoteService
    {
        public IResponse Create(CreateVoteDTO createVoteDTO);
        public IResponse GetCountOfVotes(Guid poolID);
        public IResponse AddVoteToDashboard(OptionEntity option);

    }
}
