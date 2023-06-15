using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Domain;
using VotingApp.Vote.Domain;

namespace VotingApp.Vote.Application
{
    public class VoteNotificationService : IVoteNotificationService
    {

        private readonly IVoteCountCache _voteCountCache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        public VoteNotificationService(IVoteCountCache voteCountCache, IUnitOfWork unitOfWork, ILogger<VoteNotificationService> logger)
        {
            _voteCountCache = voteCountCache;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IResponse AddVoteToDashboard(OptionEntity option)
        {
            try
            {
                var list = _voteCountCache.GetCount(option.PollID);

                if (list is null)
                {
                    list = _unitOfWork.VoteRepository.VotesByPoll(option.PollID);
                    if (list is null || !list.Any()) return new ResponseFailure("Count was not found", 404);
                }
                else
                {
                    list = list.Select(o =>
                    {
                        if (o.ID == option.ID) o.Count++;
                        return o;
                    }).ToList();
                }
                _voteCountCache.Set(option.PollID, list);
                return new ResponseSuccess(list, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return new ResponseFailure("Internal Error", 500);
            }
        }

        public IResponse GetCountOfVotes(Guid pollID)
        {
            try
            {
                var list = _voteCountCache.GetCount(pollID);

                if (list is null)
                {
                    list = _unitOfWork.VoteRepository.VotesByPoll(pollID);
                    if (list is null || !list.Any()) return new ResponseFailure("Count was not found", 404);
                }

                _voteCountCache.Set(pollID, list);
                return new ResponseSuccess(list, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return new ResponseFailure("Internal Error", 500);
            }
        }
    }
}
