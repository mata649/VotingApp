using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VotingApp.Base.Domain;
using VotingApp.User.Domain;

namespace VotingApp.User.Domain.DTO
{
    public class CreateUserDTO : CreateDTO<UserEntity>
    {

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(50)]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8), MaxLength(50)]
        public string Password { get; set; } = string.Empty;

        public override UserEntity ToEntity() => new() { Email = Email, Name = Name, Password = Password, ID = Guid.NewGuid() };

    }
}
