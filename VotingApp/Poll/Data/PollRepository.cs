using VotingApp.Base.Data;
using VotingApp.Context;
using VotingApp.Poll.Domain;

namespace VotingApp.Poll.Data;

public class PollRepository : BaseRepository<PollEntity>, IPollRepository
{
    public PollRepository(VotingAppContext context) : base(context)
    {
    }
}

