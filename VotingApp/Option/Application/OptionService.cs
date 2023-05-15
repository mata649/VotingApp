using VotingApp.Base.Domain;
using VotingApp.Pool.Domain;
using VotingApp.Option.Domain;
using VotingApp.Option.Domain.DTO;
using VotingApp.Context;

namespace VotingApp.Option.Application
{
    public class OptionService : IOptionService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public OptionService(UnitOfWork unitOfWork, ILogger<OptionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IResponse Create(CreateOptionDTO createOptionDTO)
        {
            try
            {
                OptionEntity option = createOptionDTO.ToEntity();
                PoolEntity? poolFound = _unitOfWork.PoolRepository.GetById(option.PoolID);
                if (poolFound is null) return new ResponseFailure("Pool was not found", 404);
                if (poolFound.UserID != createOptionDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);
                _unitOfWork.OptionRepository.Create(option);
                _unitOfWork.Save();
                return new ResponseSuccess(ResponseOptionDTO.FromOption(option), 201);
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
                OptionEntity? optionFound = _unitOfWork.OptionRepository.GetById(deleteDTO.ID, o => o.Pool);
                if (optionFound is null) return new ResponseFailure("Option was not found", 404);
                if (optionFound.Pool.UserID != deleteDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);
                _unitOfWork.OptionRepository.Delete(optionFound.ID);
                _unitOfWork.Save();
                return new ResponseSuccess(ResponseOptionDTO.FromOption(optionFound), 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return new ResponseFailure("Internal Error", 500);
            }
        }

        public IResponse Get(OptionFiltersDTO optionFiltersDTO)
        {
            try
            {
                (List<OptionEntity> results, ushort currentPage, ushort totalPages) = _unitOfWork.OptionRepository.Get(optionFiltersDTO.Filters, optionFiltersDTO.Pagination);

                return new ResponseSuccess(
                    new Results<ResponseOptionDTO>()
                    {
                        CurrentPage = currentPage,
                        TotalPages = totalPages,
                        Data = results.Select(ResponseOptionDTO.FromOption)
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
                OptionEntity? optionFound = _unitOfWork.OptionRepository.GetById(getByIDDTO.ID);
                if (optionFound is null) return new ResponseFailure("Option was not found", 404);

                return new ResponseSuccess(ResponseOptionDTO.FromOption(optionFound), 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return new ResponseFailure("Internal Error", 500);
            }
        }

        public IResponse Update(UpdateOptionDTO updateOptionDTO)
        {
            try
            {
                OptionEntity pool = updateOptionDTO.ToEntity();
                OptionEntity? optionFound = _unitOfWork.OptionRepository.GetById(pool.ID, o => o.Pool);
                if (optionFound is null) return new ResponseFailure("Option was not found", 404);
                if (optionFound.Pool.UserID != updateOptionDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);

                optionFound.Text = pool.Text;
                _unitOfWork.OptionRepository.Update(optionFound);
                _unitOfWork.Save();
                return new ResponseSuccess(ResponseOptionDTO.FromOption(optionFound), 200);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return new ResponseFailure("Internal Error", 500);
            }
        }
    }
}
