using Microsoft.EntityFrameworkCore.Storage;


namespace Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class;
        IGenericRepository<TEntity, int> GetRepository<TEntity>()
        where TEntity : class;

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
