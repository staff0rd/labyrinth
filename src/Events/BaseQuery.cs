using System.Threading.Tasks;

namespace Events
{
    public abstract class BaseQuery<T> 
    {
        public string ProjectionName => $"{StreamName}{this.GetType().Name}";
        public abstract string StreamName { get; }
        protected abstract string Projection();

        public Task CreateOrUpdate(EventStoreManager events)
        {
            return events.CreateOrUpdateProjection(ProjectionName, Projection());
        }
    }
}