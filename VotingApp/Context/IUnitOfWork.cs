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
        public IUserRepository UserRepository { get => UserRepository; }
        public IPollRepository PollRepository { get => PollRepository; }
        public IOptionRepository OptionRepository { get => OptionRepository; }
        public IVoteRepository VoteRepository { get => VoteRepository; }


        public void Save();

    }
}
