using VotingApp.Base.Domain;
using VotingApp.Pool.Domain;

namespace VotingApp.Pool.Domain.DTO;

public class ResponsePoolDTO : Entity
{
    public string Question { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid UserID { get; set; }
    public static ResponsePoolDTO FromPool(PoolEntity pool) => new()
    {
        ID = pool.ID,
        Question = pool.Question,
        CreatedAt = pool.CreatedAt,
        UserID = pool.UserID

    };
}

