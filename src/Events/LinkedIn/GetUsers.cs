namespace Events.LinkedIn
{
    public class GetUsers : Query<User[]>
    {
        public override string StreamName => StreamNames.LinkedIn;

        protected override string Projection()
        { 
            return @"
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