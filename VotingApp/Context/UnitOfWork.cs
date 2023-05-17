using VotingApp.Option.Data;
using VotingApp.Option.Domain;
using VotingApp.Pool.Data;
using VotingApp.Pool.Domain;
using VotingApp.User.Data;
using VotingApp.User.Domain;
using VotingApp.Vote.Data;
using VotingApp.Vote.Domain;

namespace VotingApp.Context
{
    public class UnitOfWork : IDisposable
    {
        private bool disposedValue = false;
        private readonly VotingAppContext _appContext;
        private IUserRepository? _userRepository;
        public virtual IUserRepository UserRepository { get => _userRepository ??= new UserRepository(_appContext); }

        private IPoolRepository? _poolRepository;
        public virtual IPoolRepository PoolRepository { get => _poolRepository ??= new PoolRepository(_appContext); }

        private IOptionRepository? _optionRepository;
        public virtual IOptionRepository OptionRepository { get => _optionRepository ??= new OptionRepository(_appContext); }

        private IVoteRepository? _voteRepository;
        public virtual IVoteRepository VoteRepository { get => _voteRepository ??= new VoteRepository(_appContext); }

        public UnitOfWork(VotingAppContext appContext) => _appContext = appContext;

        public void Save()
        {
            _appContext.SaveChanges();
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _appContext.Dispose();
                }


                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

       
    }
}
