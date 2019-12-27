namespace Robot
{
    public partial class Queries
    {
        public static string ApiRequests(string streamName) { return @"
fromStream('" + streamName + @"')
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