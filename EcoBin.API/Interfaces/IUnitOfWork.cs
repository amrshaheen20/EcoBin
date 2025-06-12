using EcoBin.API.Models.DbSet;

namespace EcoBin.API.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
        int Save();
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}