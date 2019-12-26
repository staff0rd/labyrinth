namespace YammerScraper
{
    public partial class Queries
    {
        public static string Groups(string streamName) { return @"
fromStream('" + streamName + @"')
.partitionBy(function(e) { return e.data.id; })
.when({
    $init:function(){
        return {};
    },
    GroupCreated: function(state, event){
        return event.data;
    }
});";
        }
    }
}