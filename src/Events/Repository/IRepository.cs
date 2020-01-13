using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Events
{
    public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        Task<Paginated<TEntity>> Paginate(Network network, int page, int pageSize, string orderBy);
        Task<TEntity> GetById(string id);
    }
}