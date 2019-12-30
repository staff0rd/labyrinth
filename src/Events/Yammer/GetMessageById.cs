using System.Threading.Tasks;
using Rest.Yammer;

namespace Events.Yammer
{
    public class GetMessageById : PartitionedQuery<Message>
    {
        public override string StreamName => StreamNames.Yammer;

        public async override Task<Message> Get(EventStoreManager events, string partition)
        {
            var result = await GetRaw(events, partition);

            if (result == "")
                return null;

            return Message.FromJson(result.ToString());
        }

        protected override string Projection()
        { return @"
fromStream('" + StreamName + @"')
.partitionBy(function(e) { return e.data.id; })
.when({
    $init:function(){
        return {};
    },
    MessageCreated: function(state, event){
        return event.data;
    }
});";
        }
    }
}