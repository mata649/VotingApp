using System.Text.Json.Serialization;

namespace VotingApp.Base.Domain;

public abstract class CreateDTO<T> where T : Entity
{

    public abstract T ToEntity();
}
public abstract class UpdateDTO<T> where T : Entity
{
    [JsonIgnore]
    public Guid ID { get; set; }
    [JsonIgnore]
    public Guid CurrentUserID { get; set; }
    public abstract T ToEntity();

}

public class DeleteDTO
{

    public Guid ID { get; set; }
    public Guid CurrentUserID { get; set; }

}

public class GetByIDDTO
{
    public Guid ID { get; set; }

}
public class GetDTO<T> where T: Entity
{
    public Filters<T> Filters { get; set; }

    public GetDTO(Filters<T> filters)
    {
        Filters = filters;
    }
}