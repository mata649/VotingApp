using VotingApp.Base.Domain;
using VotingApp.User.Domain.DTO;

namespace VotingApp.User.Domain
{
    public interface IUserService : IBaseService<UserEntity>
    {

        IResponse Get(UserFiltersDTO userFiltersDTO);
        IResponse Create(CreateUserDTO createUserDTO);
        IResponse Update(UpdateUserDTO updateUserDTO);
        IResponse Login(LoginUserDTO loginUserDTO);
        IResponse ChangePassword(ChangePasswordDTO changePasswordDTO);
 
    }
}
