namespace Web.Controllers
{
    public class SearchRequest : UserCredentialRequest
    {
        public string Search { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set;}
    }
}
