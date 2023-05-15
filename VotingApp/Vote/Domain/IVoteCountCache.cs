using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Domain
{
    public interface IVoteCountCache
    {
        public List<CountByOptionDTO>? GetCount(Guid poolID)
;
        public void Set(Guid poolID, List<CountByOptionDTO> list);
    }
}
