using System.ComponentModel.DataAnnotations;
using VotingApp.User.Domain;

namespace VotingApp.User.Domain.DTO
{
    public class LoginUserDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public UserEntity ToUser() => new() { Email = Email, Password = Password };
    }
}
