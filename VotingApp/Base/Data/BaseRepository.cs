using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VotingApp.Base.Domain;
using VotingApp.Context;

namespace VotingApp.Base.Data;

public class BaseRepository<T> : IBaseRepository<T> where T : Entity
{
    protected readonly VotingAppContext Context;
    protected readonly DbSet<T> Table;

    public BaseRepository(VotingAppContext context)
    {
        Context = context;
        Table = Context.Set<T>();
    }

    public void Create(T entity)
    {
        Console.WriteLine(entity.ToString());
        Table.Add(entity);


    }

    public void Delete(Guid id)
    {
        T? entity = Table.Find(id);

        if (entity is not null) Delete(entity);

    }
    public virtual void Delete(T entityToDelete)
    {
        if (Context.Entry(entityToDelete).State == EntityState.Detached)
        {
            Table.Attach(entityToDelete);
        }
        Table.Remove(entityToDelete);
    }


    public (List<T> results, ushort currentPage, ushort totalPages) Get(Filters<T> filters, Pagination pagination)
    {
        var query = Table.AsQueryable();

        if (filters.Filter.Any())
        {
            foreach (var filter in filters.Filter)
            {
                query = query.Where(filter);
            }
        }
        if (filters.OrderBy is not null) query = filters.OrderBy(query);



        var results = query.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();


        return (
         results,
         pagination.CurrentPage,
         (ushort)Math.Ceiling((decimal)(query.Count() / pagination.PageSize) + 1));

    }
    public (List<T> results, ushort currentPage, ushort totalPages) Get(Filters<T> filters, Pagination pagination, Expression<Func<T, object>> include)
    {
        var query = Table.AsQueryable();

        if (filters.Filter.Any())
        {
            foreach (var filter in filters.Filter)
            {
                query = query.Where(filter);
            }
        }
        if (filters.OrderBy is not null) query = filters.OrderBy(query);

        if (include is not null) query = query.Include(include);

        var results = query.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();


        return (
         results,
         pagination.CurrentPage,
         (ushort)Math.Ceiling((decimal)(query.Count() / pagination.PageSize) + 1));

    }

    public T? GetById(Guid id, Expression<Func<T, object>> include)
    {
        return Table.Include(include).FirstOrDefault(e => e.ID == id);
    }
    public T? GetById(Guid id)
    {
        return Table.Find(id);
    }
    public void Update(T entity)
    {
        Table.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;

    }


}

