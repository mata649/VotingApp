using System.Text.Json.Serialization;
using VotingApp.Option.Domain;

namespace VotingApp.Vote.Domain.DTO
{
    public class ResponseVoteDTO
    {
        public Guid UserID { get; set; }
        public Guid OptionID { get; set; }
        
        [JsonIgnore]
        public OptionEntity Option { get; set; }
        
        public static ResponseVoteDTO FromVote(VoteEntity vote) => new() { OptionID = vote.OptionID, UserID = vote.UserID, Option = vote.Option };
    }
}
