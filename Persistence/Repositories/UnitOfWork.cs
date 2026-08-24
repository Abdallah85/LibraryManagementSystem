


using Domain.Contracts;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Data;

namespace Persistence.Repositories
{
    public class UnitOfWork
        : IUnitOfWork
    {
        private readonly AppDbContext context;
        private IDbContextTransaction? _currentTransaction;
        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
        }

        private readonly Dictionary<string, object> _repositories = [];
        public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class
        {
            var typeName = typeof(TEntity).Name;
            if (_repositories.ContainsKey(typeName))
                return (IGenericRepository<TEntity, TKey>)_repositories[typeName];
            var repo = new GenericRepository<TEntity, TKey>(context);
            _repositories.Add(typeName, repo);
            return repo;
        }
        public IGenericRepository<TEntity, int> GetRepository<TEntity>() where TEntity : class
        {
            var typeName = typeof(TEntity).Name;
            if (_repositories.ContainsKey(typeName))
                return (IGenericRepository<TEntity, int>)_repositories[typeName];
            var repo = new GenericRepository<TEntity, int>(context);
            _repositories.Add(typeName, repo);
            return repo;
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _currentTransaction = await context.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            try
            {
                await SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            context.Dispose();
        }
    }
}
