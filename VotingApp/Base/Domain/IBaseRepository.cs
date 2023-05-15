using System.Linq.Expressions;
using VotingApp.User.Domain;

namespace VotingApp.Base.Domain;

public class Results<T> where T : Entity
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public ushort CurrentPage { get; set; }
    public ushort TotalPages { get; set; }

}


public class Filters<T> where T : Entity
{

    public List<Expression<Func<T, bool>>> Filter = new();
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy = null;


}

public class Pagination
{
    private ushort _currentPage = 1;
    private ushort _pageSize = 10;

    public ushort CurrentPage
    {
        get => _currentPage; set
        {
            _currentPage = (ushort)(value > 0 ? value : 1);

        }
    }
    public ushort PageSize
    {
        get => _pageSize; set
        {
            _pageSize = (ushort)(value > 0 ? value : 10);
        }
    }
}


public interface IBaseRepository<T> where T : Entity
{
    void Create(T entity);
    T? GetById(Guid id, Expression<Func<T, object>> include);
    T? GetById(Guid id);
    void Delete(Guid id);
    void Update(T entity);
    (List<T> results, ushort currentPage, ushort totalPages) Get(Filters<T> filters, Pagination pagination, Expression<Func<T, object>> include);
    (List<T> results, ushort currentPage, ushort totalPages) Get(Filters<T> filters, Pagination pagination);


}

