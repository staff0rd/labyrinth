namespace YammerScraper
{
    public partial class Queries
    {
        public static string Users(string streamName) { return @"
fromStream('" + streamName + @"')
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