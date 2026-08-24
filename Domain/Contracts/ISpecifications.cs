using System.Linq.Expressions;


namespace Domain.Contracts
{
    public interface ISpecifications<T> where T : class
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDesc { get; }
        List<Func<IQueryable<T>, IQueryable<T>>> IncludesWithThenInclude { get; set; }
        int Skip { get; }
        int Take { get; }
        bool IsPaginated { get; }

    }
}
