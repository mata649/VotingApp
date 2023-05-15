using VotingApp.Base.Domain;
using VotingApp.Pool.Domain.DTO;

namespace VotingApp.Pool.Domain;

public interface IPoolService : IBaseService<PoolEntity>
{   IResponse Get(PoolFiltersDTO poolFiltersDTO);
    IResponse Create(CreatePoolDTO createPoolDTO);
    IResponse Update(UpdatePoolDTO updatePoolDTO);
}

