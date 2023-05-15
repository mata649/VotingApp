using VotingApp.Base.Data;
using VotingApp.Context;
using VotingApp.Pool.Domain;

namespace VotingApp.Pool.Data;

public class PoolRepository : BaseRepository<PoolEntity>, IPoolRepository
{
    public PoolRepository(VotingAppContext context) : base(context)
    {
    }
}

