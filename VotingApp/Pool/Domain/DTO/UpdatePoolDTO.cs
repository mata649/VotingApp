using VotingApp.Base.Domain;
using VotingApp.Option.Domain;
using VotingApp.Pool.Domain;

namespace VotingApp.Pool.Domain.DTO;

public class UpdatePoolDTO : UpdateDTO<PoolEntity>
{
    public string Question { get; set; } = string.Empty;

    public override PoolEntity ToEntity() => new() { ID = ID, Question = Question, };


}

