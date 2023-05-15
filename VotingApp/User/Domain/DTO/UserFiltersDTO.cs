using VotingApp.Base.Domain;
using VotingApp.User.Domain;

namespace VotingApp.User.Domain.DTO
{
    public class UserFiltersDTO
    {
        public Filters<UserEntity> Filters { get; set; } = new();
        public Pagination Pagination { get; set; } = new();
        public UserFiltersDTO(ushort currentPage, ushort pageSize, string name, string email)
        {
            Pagination.CurrentPage = currentPage;
            Pagination.PageSize = pageSize;

            if (!string.IsNullOrEmpty(name)) Filters.Filter.Add(u => u.Name.Contains(name));
            if (!string.IsNullOrEmpty(email)) Filters.Filter.Add(u => u.Email.Contains(email));


        }
    }
}
