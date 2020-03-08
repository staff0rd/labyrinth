namespace Web.Controllers
{
    public class SearchRequest : QueryRequest
    {
        public string Search { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set;}
    }
}
