using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Events
{
    public class GetApiRequestsByCategory : PartitionedQuery<object>
    {
        public override string StreamName => StreamNames.ApiRequests;

        public override Task<object> Get(EventStoreManager events, string partition)
        {
            throw new System.NotImplementedException();
        }

        public async Task<DateTimeOffset?> GetOldest(EventStoreManager events, string partition) {

            var result = await GetRaw(events, partition);
            if (result == "")
                return null;

            dynamic json = JValue.Parse(result);

            DateTimeOffset? oldest = json.oldest;

            return oldest;
        }

        protected override string Projection() {
            return @"
fromStream('" + StreamName + @"')
.partitionBy(function(e) { return e.data.category; })
.when({
    $init:function(){
        return {
            count: 0,
            requests: [],
            oldest: null,
            category: null,
        };
    },
    ApiRequest: function(state, event){
        state.category = event.data.category;
        state.count += 1;
        state.requests.push(event.data.requestedAt);
        while (state.requests.length > event.data.rateLimit.requestCount) {
            state.requests.shift();
        }
        if (state.requests.length === event.data.rateLimit.requestCount) {
            state.oldest = state.requests[0];
        }
    }
});";
        }
    }
}