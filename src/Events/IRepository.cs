using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Events
{
    public interface IExternalEntityRepository<TEntity> : IRepository<TEntity, Guid> where TEntity : class, IExternalEntity
    {
        Task<TEntity> GetByExternalId(Network network, string id);
    }

    public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        Task<Paginated<TEntity>> Paginate(Network network, int page = 0, int pageSize = 200);
        Task<TEntity> GetById(Guid id);
    }
}