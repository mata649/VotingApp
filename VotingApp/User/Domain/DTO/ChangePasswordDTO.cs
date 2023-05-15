using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VotingApp.User.Domain;

namespace VotingApp.User.Domain.DTO
{
    public class ChangePasswordDTO
    {
        [JsonIgnore]
        public Guid ID { get; set; }

        [JsonIgnore]
        public Guid CurrentUserID { get; set; }

        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(50)]
        public string NewPassword { get; set; } = string.Empty;

        public UserEntity ToUser() => new() { ID = ID, Password = OldPassword };
    }
}
