namespace Events.LinkedIn
{
    public partial class Queries
    {
        public static string StreamName => "LinkedIn";
        private readonly EventStoreManager _eventStore;

        public Queries(EventStoreManager eventStore)
        {
            _eventStore = eventStore;
        }
    }
}