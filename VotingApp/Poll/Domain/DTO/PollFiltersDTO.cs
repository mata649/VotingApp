using VotingApp.Base.Domain;

namespace VotingApp.Poll.Domain.DTO
{
    public class PollFiltersDTO
    {
        public Filters<PollEntity> Filters { get; set; } = new();
        public Pagination Pagination { get; set; } = new();

        public PollFiltersDTO(ushort currentPage, ushort pageSize, string question, Guid userID)
        {
            Pagination.CurrentPage = currentPage;
            Pagination.PageSize = pageSize;

            if (!string.IsNullOrEmpty(question)) Filters.Filter.Add(p => p.Question.Contains(question));
            if (userID != Guid.Empty) Filters.Filter.Add(p => p.UserID == userID);

        }
    }
}
