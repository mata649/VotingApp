
using VotingApp.Base.Data;
using VotingApp.Context;
using VotingApp.User.Domain;

namespace VotingApp.User.Data;

public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    public UserRepository(VotingAppContext context) : base(context)
    {
    }

    public UserEntity? GetByEmail(string email)
    {
        return Table.FirstOrDefault(t => t.Email == email);
    }
}

