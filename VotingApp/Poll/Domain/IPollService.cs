using VotingApp.Base.Domain;
using VotingApp.Poll.Domain.DTO;

namespace VotingApp.Poll.Domain;

public interface IPollService : IBaseService<PollEntity>
{   IResponse Get(PollFiltersDTO pollFiltersDTO);
    IResponse Create(CreatePollDTO createpollDTO);
    IResponse Update(UpdatePollDTO updatepollDTO);
}

