namespace Web.Controllers
{
    public partial class YammerController
    {
        public class IdRequest : UserCredentialRequest
        {
            public string Id { get; set; }
        }
    }
}
