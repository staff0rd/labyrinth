namespace Web.Controllers
{
    public class CredentialRequest : UserCredentialRequest
    {
        public string ExternalIdentifier { get; set; }
        public string ExternalSecret { get; set; }
    }
}
