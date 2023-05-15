using VotingApp.Base.Domain;
using VotingApp.Option.Domain.DTO;

namespace VotingApp.Option.Domain
{
    public interface IOptionService : IBaseService<OptionEntity>
    {
        IResponse Create(CreateOptionDTO createOptionDTO);
        IResponse Update(UpdateOptionDTO updateOptionDTO);
        IResponse Get(OptionFiltersDTO optionFiltersDTO);
    }
}
