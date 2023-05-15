using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VotingApp.Base.Domain;
using VotingApp.User.Domain;

namespace VotingApp.User.Domain.DTO
{
    public class UpdateUserDTO : UpdateDTO<UserEntity>
    {

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        public override UserEntity ToEntity() => new() { ID = ID, Name = Name, Email = Email };


    }
}
