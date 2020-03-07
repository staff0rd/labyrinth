namespace Web.Controllers
{
    public partial class YammerController
    {
        public class SearchRequest : UserCredentialRequest
        {
            public string Search { get; set; }

            public int PageSize { get; set; }

            public int PageNumber { get; set;}
        }
    }
}
