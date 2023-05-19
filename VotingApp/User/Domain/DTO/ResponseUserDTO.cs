using VotingApp.Base.Domain;
using VotingApp.Poll.Domain;
using VotingApp.User.Domain;
using VotingApp.Vote.Domain;

namespace VotingApp.User.Domain.DTO;

public class ResponseUserDTO : Entity
{

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public static ResponseUserDTO FromUser(UserEntity user) => new() { ID = user.ID, Name = user.Name, Email = user.Email };
}

