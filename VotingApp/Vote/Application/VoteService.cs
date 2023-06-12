using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Domain;
using VotingApp.User.Domain;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Application
{
    public class VoteService : IVoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public VoteService(IUnitOfWork unitOfWork, ILogger<VoteService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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


    }
}
