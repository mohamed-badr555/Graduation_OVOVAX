using System;
using System.Threading.Tasks;
using OVOVAX.Core.Entities;

namespace OVOVAX.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<int> Complete();
    }
}
