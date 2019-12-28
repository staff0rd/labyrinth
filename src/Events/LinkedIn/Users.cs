using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Events.LinkedIn
{
    public partial class Queries
    {
        public async Task<User> GetUser(string id)
        {
            var result = await _eventStore.GetProjection($"{StreamName}{nameof(UserById)}", id);

            if (result == "")
                return null;

            return User.FromJson(result.ToString());
        }

        public async Task<User[]> GetUsers()
        {
            var result = await _eventStore.GetProjection($"{StreamName}{nameof(AllUsers)}");
            
            return JsonConvert.DeserializeObject<User[]>(result);
        }

        public string UserById() { return @"
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

        public string AllUsers() { return @"
fromStream('" + StreamName + @"')
.when({
    $init:function(){
        return [];
    },
    UserCreated: function(state, event){
        const filtered = state.filter(u => (u.id !== event.data.id)); 
        filtered.push(event.data);
        return filtered;
    }
});";
        }
    }
}