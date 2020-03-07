namespace Web.Controllers
{
    public partial class YammerController
    {
        public class TokenRequest : UserCredentialRequest
        {
            public string Token { get; set; }
        }
    }
}
