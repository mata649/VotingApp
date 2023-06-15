using System.Linq.Expressions;
using VotingApp.User.Domain;

namespace VotingApp.Base.Domain;

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

