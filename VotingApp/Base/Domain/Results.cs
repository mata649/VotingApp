namespace VotingApp.Base.Domain
{
    public class Results<T> where T : Entity
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public ushort CurrentPage { get; set; }
        public ushort TotalPages { get; set; }

    }

}
