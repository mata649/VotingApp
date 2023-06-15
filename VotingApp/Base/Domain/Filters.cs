using System.Linq.Expressions;

namespace VotingApp.Base.Domain
{
    public class Filters<T> where T : Entity
    {

        public List<Expression<Func<T, bool>>> Filter = new();
        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy = null;

    }

}
