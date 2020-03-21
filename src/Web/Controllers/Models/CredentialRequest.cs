namespace Web.Controllers
{
    public class CredentialRequest : QueryRequest
    {
        public string ExternalIdentifier { get; set; }
        public string ExternalSecret { get; set; }
    }
}
