using VotingApp.Base.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Domain
{
    public interface IVoteRepository : IBaseRepository<VoteEntity>
    {
        public List<CountByOptionDTO> VotesByPoll(Guid pollID);
    }
}
