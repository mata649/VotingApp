namespace VotingApp.Base.Domain;

public interface IResponse
{
    object Value { get; set; }
    int Type { get; set; }

}

public class ResponseSuccess : IResponse
{
    public object Value { get; set; }
    public int Type { get; set; }

    public ResponseSuccess(object value, int type)
    {
        Value = value;
        Type = type;
    }
}
public class Error
{
    public string Message { get; set; } = string.Empty;
}

public class ResponseFailure : IResponse
{

    public object Value { get; set; }
    public int Type { get; set; }
    public ResponseFailure(string value, int type)
    {
        Value = new Error() { Message = value };
        Type = type;
    }
}