namespace YammerScraper
{
    public partial class Queries
    {
        public static string Threads(string streamName) { return @"
fromStream('" + streamName + @"')
.partitionBy(function(e) { return e.data.id; })
.when({
    $init:function(){
        return {};
    },
    ThreadCreated: function(state, event){
        return event.data;
    }
});";
        }
    }
}