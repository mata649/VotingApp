using VotingApp.Base.Domain;
using VotingApp.Option.Domain;

namespace VotingApp.Option.Domain.DTO
{
    public class ResponseOptionDTO : Entity
    {
        public string Text { get; set; } = string.Empty;
        public Guid PoolID { get; set; }
        public static ResponseOptionDTO FromOption(OptionEntity option) => new() { ID = option.ID, Text = option.Text, PoolID = option.PoolID };
    }
}
