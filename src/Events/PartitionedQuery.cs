using System.Threading.Tasks;

namespace Events
{
    public abstract class PartitionedQuery<T> : BaseQuery<T>
    {
        public abstract Task<T> Get(EventStoreManager events, string partition);
        public Task<string> GetRaw(EventStoreManager events, string partition)
        {
            return events.GetProjection($"{StreamName}{this.GetType().Name}", partition);
        }
    }
}