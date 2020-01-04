using System.Threading.Tasks;
using Newtonsoft.Json;
using Rest.Yammer;

namespace Events.Yammer
{
    public class GetMessages : Query<Message[]>
{
        public override string StreamName => StreamNames.Yammer;

        protected override string Projection()
        {
            return @"
fromStream('" + StreamName + @"')
.when({
    $init:function(){
        return [];
    },
    MessageCreated: function(state, event){
        const filtered = state.filter(u => (u.id !== event.data.id)); 
        filtered.push(event.data);
        return filtered;
    }
});";
        }
    }
}