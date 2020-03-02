using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Events
{
    public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        Task<TEntity> GetById(string userName, string id);
    }
}