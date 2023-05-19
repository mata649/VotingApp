using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Domain;
using VotingApp.Poll.Domain;
using VotingApp.Poll.Domain.DTO;

namespace VotingApp.Poll.Application;

public class PollService : IPollService
{
    private readonly ILogger _logger;
    private readonly UnitOfWork _unitOfWork;

    public PollService(UnitOfWork unitOfWork, ILogger<PollService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;

    }

    public IResponse Create(CreatePollDTO createPollDTO)
    {

        try
        {
            PollEntity poll = createPollDTO.ToEntity();

            List<OptionEntity> options = createPollDTO.GetOptions(poll.ID);

            if (_unitOfWork.UserRepository.GetById(poll.UserID) is null) return new ResponseFailure("User does not exist", 404);

            _unitOfWork.PollRepository.Create(poll);
            if (options.Any()) options.ForEach(_unitOfWork.OptionRepository.Create);

            _unitOfWork.Save();
            return new ResponseSuccess(ResponsePollDTO.FromPoll(poll), 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);

            return new ResponseFailure("Internal Error", 500);
        }

    }
    public IResponse Update(UpdatePollDTO updatePollDTO)
    {
        try
        {
            PollEntity poll = updatePollDTO.ToEntity();
            PollEntity? pollFound = _unitOfWork.PollRepository.GetById(poll.ID);
            if (pollFound is null) return new ResponseFailure("Poll was not found", 404);

            if (pollFound.UserID != updatePollDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);
            pollFound.Question = poll.Question;
            _unitOfWork.PollRepository.Update(pollFound);
            _unitOfWork.Save();
            return new ResponseSuccess(ResponsePollDTO.FromPoll(pollFound), 200);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }
    public IResponse Delete(DeleteDTO deleteDTO)
    {
        try
        {
            PollEntity? pollFound = _unitOfWork.PollRepository.GetById(deleteDTO.ID);
            if (pollFound is null) return new ResponseFailure("Poll was not found", 404);
            if (pollFound.UserID != deleteDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);
            _unitOfWork.PollRepository.Delete(pollFound.ID);
            _unitOfWork.Save();
            return new ResponseSuccess(ResponsePollDTO.FromPoll(pollFound), 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }

    public IResponse Get(PollFiltersDTO pollFiltersDTO)
    {
        try
        {
            (List<PollEntity> results, ushort currentPage, ushort totalPages) = _unitOfWork.PollRepository.Get(pollFiltersDTO.Filters, pollFiltersDTO.Pagination);

            return new ResponseSuccess(
                new Results<ResponsePollDTO>()
                {
                    CurrentPage = currentPage,
                    TotalPages = totalPages,
                    Data = results.Select(ResponsePollDTO.FromPoll)
                }
                , 200);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);

        }
    }

    public IResponse GetById(GetByIDDTO getByIDDTO)
    {
        try
        {
            PollEntity? pollFound = _unitOfWork.PollRepository.GetById(getByIDDTO.ID);
            if (pollFound is null) return new ResponseFailure("Poll was not found", 404);

            return new ResponseSuccess(ResponsePollDTO.FromPoll(pollFound), 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }


}

