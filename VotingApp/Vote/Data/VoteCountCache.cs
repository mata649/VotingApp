using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Data
{
    public class VoteCountCache : IVoteCountCache
    {
        private readonly Dictionary<Guid, List<CountByOptionDTO>> _cache = new();

        public List<CountByOptionDTO>? GetCount(Guid poolID)
        {
            _cache.TryGetValue(poolID, out var list);
            return list;
        }

        public void Set(Guid poolID, List<CountByOptionDTO> list)
        {
            _cache[poolID] = list;
        }

    }
}
