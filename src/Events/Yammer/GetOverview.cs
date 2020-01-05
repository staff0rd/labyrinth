using System.Threading.Tasks;
using Newtonsoft.Json;
using Rest.Yammer;

namespace Events.Yammer
{
    public class GetOverview : Query<Overview>
{
        public override string StreamName => StreamNames.Yammer;

        protected override string Projection()
        {
            return @"
fromStream('" + StreamName + @"')
.when({
    $init:function(){
        return {
            messages: 0,
            users: 0,
            threads: 0,
            groups: 0
        };
    },
    MessageCreated: function(state, event){
        state.messages++;
    },
    UserCreated: function(state, event){
        state.users++;
    },
    GroupCreated: function(state, event){
        state.groups++;
    },
    ThreadCreated: function(state, event){
        state.threads++;
    },
});";
        }
    }
}