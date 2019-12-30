using System.Threading.Tasks;
using Rest.Yammer;

namespace Events.Yammer
{
    public class GetUserById : PartitionedQuery<User>
    {
        public override string StreamName => StreamNames.Yammer;

        public async override Task<User> Get(EventStoreManager events, string partition)
        {
            var result = await GetRaw(events, partition);

            if (result == "")
                return null;

            return User.FromJson(result.ToString());
        }

        protected override string Projection()
        { return @"
fromStream('" + StreamName + @"')
.partitionBy(function(e) { return e.data.id; })
.when({
    $init:function(){
        return {};
    },
    UserCreated: function(state, event){
        return event.data;
    }
});";
        }
    }
}