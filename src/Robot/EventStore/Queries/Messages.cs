namespace Robot
{
    public partial class Queries
    {
        public static string Messages(string streamName) { return @"
fromStream('" + streamName + @"')
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