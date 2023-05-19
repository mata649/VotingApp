using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Domain
{
    public interface IVoteCountCache
    {
        public List<CountByOptionDTO>? GetCount(Guid pollID)
;
        public void Set(Guid pollID, List<CountByOptionDTO> list);
    }
}
