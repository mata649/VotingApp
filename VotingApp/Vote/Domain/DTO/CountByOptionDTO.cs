namespace VotingApp.Vote.Domain.DTO
{
    public class CountByOptionDTO
    {
        public Guid ID { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
    }
}
