using System.ComponentModel.DataAnnotations;
using VotingApp.Base.Domain;
using VotingApp.Option.Domain;

namespace VotingApp.Option.Domain.DTO
{
    public class UpdateOptionDTO : UpdateDTO<OptionEntity>
    {
        [Required, MaxLength(60)]
        public string Text { get; set; } = string.Empty;

        public override OptionEntity ToEntity() => new() { ID = ID, Text = Text, };
    }
}
