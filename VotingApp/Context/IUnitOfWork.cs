using VotingApp.Option.Data;
using VotingApp.Option.Domain;
using VotingApp.Poll.Data;
using VotingApp.Poll.Domain;
using VotingApp.User.Data;
using VotingApp.User.Domain;
using VotingApp.Vote.Data;
using VotingApp.Vote.Domain;

namespace VotingApp.Context
{
    public interface IUnitOfWork
    {
        public virtual IUserRepository UserRepository { get => UserRepository; }

        public virtual IPollRepository PollRepository { get => PollRepository; }

        public virtual IOptionRepository OptionRepository { get => OptionRepository; }

        public virtual IVoteRepository VoteRepository { get => VoteRepository; }


        public void Save();
        
    }
}
