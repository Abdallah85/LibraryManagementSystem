using Domain.Contracts;
using System.Linq.Expressions;

namespace Services.Specifications;
public abstract class BaseSpecifications<T>(Expression<Func<T, bool>>? criteria)
    : ISpecifications<T>
    where T : class
{
    public Expression<Func<T, bool>> Criteria { get; } = criteria!;
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public List<Func<IQueryable<T>, IQueryable<T>>> IncludesWithThenInclude { get; set; } = new List<Func<IQueryable<T>, IQueryable<T>>>();
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDesc { get; private set; }

    public int Skip { get; private set; }
    public int Take { get; private set; }
    public bool IsPaginated { get; private set; }
    protected void ApplyPagination(int pageSize, int pageIndex)
    {
        IsPaginated = true;
        Take = pageSize;
        Skip = (pageIndex - 1) * pageSize;
    }
    public BaseSpecifications<T> AddInclude(Expression<Func<T, object>> expression)
    {
        Includes.Add(expression);
        return this;
    }

    public BaseSpecifications<T> AddIncludesWithThenInclude(Func<IQueryable<T>, IQueryable<T>> func)
    {
        IncludesWithThenInclude.Add(func);
        return this;
    }

    public BaseSpecifications<T> AddInclude(List<Expression<Func<T, object>>> expressions)
    {
        Includes.AddRange(expressions);
        return this;
    }

    protected BaseSpecifications<T> AddOrderBy(Expression<Func<T, object>> expression)
    {
        OrderBy = expression;
        return this;
    }

    public BaseSpecifications<T> AddOrderByDesc(Expression<Func<T, object>> expression)
    {
        OrderByDesc = expression;
        return this;
    }
}
