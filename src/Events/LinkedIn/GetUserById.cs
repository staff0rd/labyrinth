using System.Threading.Tasks;

namespace Events.LinkedIn
{
    public class GetUserById : PartitionedQuery<User>
    {
        public override string StreamName => StreamNames.LinkedIn;

        public async override Task<User> Get(EventStoreManager events, string partition)
        {
            var result = await GetRaw(events, partition);

            if (result == "")
                return null;

            return User.FromJson(result.ToString());
        }

        protected override string Projection()
        { 
            return @"
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