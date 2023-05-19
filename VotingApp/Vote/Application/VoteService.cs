using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Domain;
using VotingApp.Poll.Domain;
using VotingApp.User.Domain;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Application
{
    public class VoteService : IVoteService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IVoteCountCache _voteCountCache;

        public VoteService(UnitOfWork unitOfWork, ILogger<VoteService> logger, IVoteCountCache voteCountCache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _voteCountCache = voteCountCache;
        }

        public IResponse Create(CreateVoteDTO createVoteDTO)
        {
            try
            {
                VoteEntity vote = createVoteDTO.ToEntity();


                OptionEntity? optionFound = _unitOfWork.OptionRepository.GetById(vote.OptionID);
                if (optionFound is null) return new ResponseFailure("Option was not found", 404);

                UserEntity? userFound = _unitOfWork.UserRepository.GetById(vote.UserID);
                if (userFound is null) return new ResponseFailure("User was not found", 404);

                // Check if the user has voted to avoid multiple votes of the same user in the same poll
                Filters<VoteEntity> filters = new();
                filters.Filter.Add(v => v.Option.PollID == optionFound.PollID && v.UserID == userFound.ID);

                (List<VoteEntity> results, _, _) = _unitOfWork.VoteRepository.Get(filters, new Pagination());

                if (results.Any()) return new ResponseFailure("You can't participate again in this poll", 403);

                _unitOfWork.VoteRepository.Create(vote);
                _unitOfWork.Save();

                vote.Option = optionFound;

                return new ResponseSuccess(ResponseVoteDTO.FromVote(vote), 201);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return new ResponseFailure("Internal Error", 500);
            }
        }

        public IResponse AddVoteToDashboard(OptionEntity option)
        {
            try
            {
                var list = _voteCountCache.GetCount(option.PollID);
              
                if (list is null)
                {
                    list = _unitOfWork.VoteRepository.VotesBypoll(option.PollID);
                    if (!list.Any()) return new ResponseFailure("Count was not found", 404);
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
                    list = _unitOfWork.VoteRepository.VotesBypoll(pollID);
                    if (!list.Any()) return new ResponseFailure("Count was not found", 404);
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
