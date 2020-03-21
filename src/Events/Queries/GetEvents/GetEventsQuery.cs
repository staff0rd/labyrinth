namespace Events
{
    public class GetEventsQuery : GetCollectionPagedById<Event>
    {
        public string[] EventTypes { get; set; }
    }
}