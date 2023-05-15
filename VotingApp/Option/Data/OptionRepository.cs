using VotingApp.Base.Data;
using VotingApp.Context;
using VotingApp.Option.Domain;

namespace VotingApp.Option.Data
{
    public class OptionRepository : BaseRepository<OptionEntity>, IOptionRepository
    {
        public OptionRepository(VotingAppContext context) : base(context)
        {
        }
    }
}
