using System.Collections.Concurrent;

namespace Events
{
    public class Credential
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ExternalIdentifier { get; set; }
        public string ExternalSecret { get; set;}
    }

    public class CredentialCache
    {
        private ConcurrentDictionary<string, Credential> _credentials = new ConcurrentDictionary<string, Credential>();

        public void Add(Network network, Credential credential)
        {
            var key = $"{network}/{credential.Username}";
            if (_credentials.ContainsKey(key))
                _credentials.TryRemove(key, out var _);
            
            _credentials.TryAdd(key, credential);
        }

        public Credential Get(Network network, string userName)
        {
            var key = $"{network}/{userName}";
            if (_credentials.TryGetValue(key, out var result))
                return result;
            throw new System.Exception("No credential");
        }
    }
}