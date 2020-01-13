using System;
using System.Threading.Tasks;

namespace Events
{
    public interface IExternalEntityRepository<TEntity> : IRepository<TEntity, Guid> where TEntity : class, IExternalEntity
    {
        Task<TEntity> GetByExternalId(Network network, string id);
    }
}