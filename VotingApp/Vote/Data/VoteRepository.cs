using Microsoft.EntityFrameworkCore;
using VotingApp.Base.Data;
using VotingApp.Context;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Data
{
    public class VoteRepository : BaseRepository<VoteEntity>, IVoteRepository
    {
        public VoteRepository(VotingAppContext context) : base(context)
        {
        }

        public List<CountByOptionDTO> VotesByPoll(Guid pollID)
        {
            var result = from option in Context.Options
                         where option.PollID == pollID
                         join vote in Context.Votes on option.ID equals vote.OptionID into optionVotes
                         select new CountByOptionDTO
                         {
                             ID = option.ID,
                             Text = option.Text,
                             Count = optionVotes.Count()
                         };
            return result.ToList();
        }
    }
}
