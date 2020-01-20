using System;
using System.Threading.Tasks;

namespace Events
{
    public interface IExternalEntityRepository<TEntity> : IRepository<TEntity, string> where TEntity : class, IExternalEntity
    {
        Task<TEntity> GetByExternalId(Network network, string id);
    }
}