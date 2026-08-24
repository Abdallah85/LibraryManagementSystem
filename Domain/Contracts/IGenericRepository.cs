using System.Linq.Expressions;


namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class
    {
        IQueryable<TEntity> Query();
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task<TEntity?> GetAsync(TKey key);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetAsync(ISpecifications<TEntity> specifications);
        Task<int> CountAsync(ISpecifications<TEntity> specifications);
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity> specifications);
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(ISpecifications<TEntity> specifications, Expression<Func<TEntity, TResult>> projection);
        Task<TResult?> GetAsync<TResult>(ISpecifications<TEntity> specifications, Expression<Func<TEntity, TResult>> projection);
    }
}
