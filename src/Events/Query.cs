using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Events
{
    public abstract class Query<T> : BaseQuery<T>
    {
        public async virtual Task<T> Get(EventStoreManager events)
        {
            var result = await GetRaw(events);
            
            return JsonConvert.DeserializeObject<T>(result);
        }

        public Task<string> GetRaw(EventStoreManager events)
        {
            return events.GetProjection(ProjectionName);
        }
    }
}