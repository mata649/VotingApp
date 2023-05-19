using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VotingApp.Base.Domain;
using VotingApp.Option.Domain;

namespace VotingApp.Poll.Domain.DTO;

public class PollOption
{

    [Required, MaxLength(60)]
    public string Text { get; set; } = string.Empty;
    public OptionEntity ToEntity(Guid pollID) => new()
    {
        ID = Guid.NewGuid(),
        PollID = pollID,
        Text = Text
    };
}
public class CreatePollDTO : CreateDTO<PollEntity>
{

    public string Question { get; set; } = string.Empty;
    [JsonIgnore]
    public Guid UserID { get; set; }

    public virtual IEnumerable<PollOption> Options { get; set; } = Enumerable.Empty<PollOption>();


    public override PollEntity ToEntity() => new()
    {
        ID = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Question = Question,
        UserID = UserID,

    };


    public List<OptionEntity> GetOptions(Guid pollID) => Options.Select(o => o.ToEntity(pollID)).ToList();

}

