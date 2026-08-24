using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System.Linq.Expressions;

namespace Persistence.Repositories;
public class GenericRepository<TEntity, TKey>(AppDbContext context)
    : IGenericRepository<TEntity, TKey>
    where TEntity : class
{
    public IQueryable<TEntity> Query() => context.Set<TEntity>().AsQueryable();
    public void Add(TEntity entity) => context.Set<TEntity>().Add(entity);
    public void Delete(TEntity entity) => context.Set<TEntity>().Remove(entity);
    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);
    public async Task<IEnumerable<TEntity>> GetAllAsync()
        => await context.Set<TEntity>().ToListAsync();
    public async Task<TEntity?> GetAsync(TKey key)
        => await context.Set<TEntity>().FindAsync(key);
    public async Task<TEntity?> GetAsync(ISpecifications<TEntity> specifications)
        => await SpecificationsEvaluator.CreateQuery(context.Set<TEntity>(), specifications)
            .FirstOrDefaultAsync();
    public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity> specifications)
       => await SpecificationsEvaluator.CreateQuery(context.Set<TEntity>(), specifications)
            .ToListAsync();
    public async Task<int> CountAsync(ISpecifications<TEntity> specifications)
          => await SpecificationsEvaluator.CreateQuery(context.Set<TEntity>(), specifications)
            .CountAsync();
    public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(ISpecifications<TEntity> specifications, Expression<Func<TEntity, TResult>> projection)
        => await SpecificationsEvaluator.CreateQuery(context.Set<TEntity>(), specifications).Select(projection).ToListAsync();

    public async Task<TResult?> GetAsync<TResult>(ISpecifications<TEntity> specifications, Expression<Func<TEntity, TResult>> projection)
        => await SpecificationsEvaluator.CreateQuery(context.Set<TEntity>(), specifications).Select(projection)
             .FirstOrDefaultAsync();

    public void AddRange(IEnumerable<TEntity> entities)
        =>  context.Set<TEntity>().AddRange(entities);

    public void DeleteRange(IEnumerable<TEntity> entities)
        => context.Set<TEntity>().RemoveRange(entities);
}
