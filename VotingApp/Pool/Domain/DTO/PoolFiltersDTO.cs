using System.Linq.Expressions;
using VotingApp.Base.Domain;
using VotingApp.Pool.Domain;

namespace VotingApp.Pool.Domain.DTO
{
    public class PoolFiltersDTO
    {
        public Filters<PoolEntity> Filters { get; set; } = new();
        public Pagination Pagination { get; set; } = new();

        public PoolFiltersDTO(ushort currentPage, ushort pageSize, string question, Guid userID)
        {
            Pagination.CurrentPage = currentPage;
            Pagination.PageSize = pageSize;

            if (!string.IsNullOrEmpty(question)) Filters.Filter.Add(p => p.Question.Contains(question));
            if (userID != Guid.Empty) Filters.Filter.Add(p => p.UserID == userID);

        }
    }
}
