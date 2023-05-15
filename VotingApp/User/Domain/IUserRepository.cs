using VotingApp.Base.Domain;

namespace VotingApp.User.Domain;
public interface IUserRepository : IBaseRepository<UserEntity>
{
    UserEntity? GetByEmail(string email);
}

