using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VotingApp.Base.Domain;
using VotingApp.Option.Domain;
using VotingApp.Pool.Domain;

namespace VotingApp.Pool.Domain.DTO;

public class PoolOption
{

    [Required, MaxLength(60)]
    public string Text { get; set; } = string.Empty;
    public OptionEntity ToEntity(Guid poolID) => new()
    {
        ID = Guid.NewGuid(),
        PoolID = poolID,
        Text = Text
    };
}
public class CreatePoolDTO : CreateDTO<PoolEntity>
{

    public string Question { get; set; } = string.Empty;
    [JsonIgnore]
    public Guid UserID { get; set; }

    public virtual IEnumerable<PoolOption> Options { get; set; } = Enumerable.Empty<PoolOption>();


    public override PoolEntity ToEntity() => new()
    {
        ID = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Question = Question,
        UserID = UserID,

    };


    public List<OptionEntity> GetOptions(Guid poolID) => Options.Select(o => o.ToEntity(poolID)).ToList();

}

