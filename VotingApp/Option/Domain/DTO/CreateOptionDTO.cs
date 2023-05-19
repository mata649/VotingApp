using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VotingApp.Base.Domain;
using VotingApp.Option.Domain;

namespace VotingApp.Option.Domain.DTO
{
    public class CreateOptionDTO : CreateDTO<OptionEntity>
    {
        [Required]
        public Guid PollID { get; set; }
        [Required, MaxLength(60)]
        public string Text { get; set; } = string.Empty;
        [JsonIgnore]
        public Guid CurrentUserID { get; set; }
        public override OptionEntity ToEntity() => new()
        {
            ID = Guid.NewGuid(),
            PollID = PollID,
            Text = Text
        };

    }
}
