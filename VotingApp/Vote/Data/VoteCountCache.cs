using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Data
{
    public class VoteCountCache : IVoteCountCache
    {
        private readonly Dictionary<Guid, List<CountByOptionDTO>> _cache = new();

        public List<CountByOptionDTO>? GetCount(Guid pollID)
        {
            _cache.TryGetValue(pollID, out var list);
            return list;
        }

        public void Set(Guid pollID, List<CountByOptionDTO> list)
        {
            _cache[pollID] = list;
        }

    }
}
