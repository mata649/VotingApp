using VotingApp.Base.Domain;
using VotingApp.Option.Domain;

namespace VotingApp.Option.Domain.DTO
{
    public class OptionFiltersDTO
    {
        public Filters<OptionEntity> Filters { get; set; } = new();
        public Pagination Pagination { get; set; } = new();

        public OptionFiltersDTO(ushort currentPage, ushort pageSize, string text, Guid poolID)
        {
            Pagination.CurrentPage = currentPage;
            Pagination.PageSize = pageSize;

            if (!string.IsNullOrEmpty(text)) Filters.Filter.Add(o => o.Text == text);
            if (poolID != Guid.Empty) Filters.Filter.Add(o => o.PoolID == poolID);

        }
    }
}
