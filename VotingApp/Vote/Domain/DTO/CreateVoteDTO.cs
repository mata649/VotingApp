using System.Text.Json.Serialization;
using VotingApp.Base.Domain;

namespace VotingApp.Vote.Domain.DTO
{
    public class CreateVoteDTO : CreateDTO<VoteEntity>
    {
        [JsonIgnore]
        public Guid UserID { get; set; }
        public Guid OptionID { get; set; }

        public override VoteEntity ToEntity() => new() { ID = Guid.NewGuid(), UserID = UserID, OptionID = OptionID };

    }
}
