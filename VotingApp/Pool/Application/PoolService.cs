using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Domain;
using VotingApp.Pool.Domain;
using VotingApp.Pool.Domain.DTO;

namespace VotingApp.Pool.Application;

public class PoolService : IPoolService
{
    private readonly ILogger _logger;
    private readonly UnitOfWork _unitOfWork;

    public PoolService(UnitOfWork unitOfWork, ILogger<PoolService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;

    }

    public IResponse Create(CreatePoolDTO createPoolDTO)
    {

        try
        {
            PoolEntity pool = createPoolDTO.ToEntity();

            List<OptionEntity> options = createPoolDTO.GetOptions(pool.ID);

            if (_unitOfWork.UserRepository.GetById(pool.UserID) is null) return new ResponseFailure("User does not exist", 404);

            _unitOfWork.PoolRepository.Create(pool);
            if (options.Any()) options.ForEach(_unitOfWork.OptionRepository.Create);

            _unitOfWork.Save();
            return new ResponseSuccess(ResponsePoolDTO.FromPool(pool), 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);

            return new ResponseFailure("Internal Error", 500);
        }

    }
    public IResponse Update(UpdatePoolDTO updatePoolDTO)
    {
        try
        {
            PoolEntity pool = updatePoolDTO.ToEntity();
            PoolEntity? poolFound = _unitOfWork.PoolRepository.GetById(pool.ID);
            if (poolFound is null) return new ResponseFailure("Pool was not found", 404);

            if (poolFound.UserID != updatePoolDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);
            poolFound.Question = pool.Question;
            _unitOfWork.PoolRepository.Update(poolFound);
            _unitOfWork.Save();
            return new ResponseSuccess(ResponsePoolDTO.FromPool(poolFound), 200);

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
            PoolEntity? poolFound = _unitOfWork.PoolRepository.GetById(deleteDTO.ID);
            if (poolFound is null) return new ResponseFailure("Pool was not found", 404);
            if (poolFound.UserID != deleteDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);
            _unitOfWork.PoolRepository.Delete(poolFound.ID);
            _unitOfWork.Save();
            return new ResponseSuccess(ResponsePoolDTO.FromPool(poolFound), 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }

    public IResponse Get(PoolFiltersDTO poolFiltersDTO)
    {
        try
        {
            (List<PoolEntity> results, ushort currentPage, ushort totalPages) = _unitOfWork.PoolRepository.Get(poolFiltersDTO.Filters, poolFiltersDTO.Pagination);

            return new ResponseSuccess(
                new Results<ResponsePoolDTO>()
                {
                    CurrentPage = currentPage,
                    TotalPages = totalPages,
                    Data = results.Select(ResponsePoolDTO.FromPool)
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
            PoolEntity? poolFound = _unitOfWork.PoolRepository.GetById(getByIDDTO.ID);
            if (poolFound is null) return new ResponseFailure("Pool was not found", 404);

            return new ResponseSuccess(ResponsePoolDTO.FromPool(poolFound), 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }


}

