using System.Collections.Concurrent;

namespace Events
{
    public class YammerCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
    public class CredentialCache
    {
        public ConcurrentDictionary<string, YammerCredential> Yammer = new ConcurrentDictionary<string, YammerCredential>();
    }
}