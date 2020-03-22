using System;
using System.Collections.Concurrent;

namespace Events
{
    public class Credential
    {
        public Credential() {}
        public Credential(string username, string password) 
        {
            Username = username;
            Password = password;
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ExternalIdentifier { get; set; }
        public string ExternalSecret { get; set;}
    }

    public class CredentialCache
    {
        private ConcurrentDictionary<string, Credential> _credentials = new ConcurrentDictionary<string, Credential>();
        
        public void Add(string self, Credential credential)
        {
            var key = $"{self}/{credential.Username}";
            if (_credentials.ContainsKey(key))
                _credentials.TryRemove(key, out var _);
            
            _credentials.TryAdd(key, credential);
        }

        public void Add(Guid sourceId, Credential credential)
        {
            var key = $"{sourceId}/{credential.Username}";
            if (_credentials.ContainsKey(key))
                _credentials.TryRemove(key, out var _);
            
            _credentials.TryAdd(key, credential);
        }

        public Credential Get(Guid sourceId, string userName)
        {
            var key = $"{sourceId}/{userName}";
            if (_credentials.TryGetValue(key, out var result))
                return result;
            throw new System.Exception("No credential");
        }

        public Credential Get(string sourceId, string userName)
        {
            var key = $"{sourceId}/{userName}";
            if (_credentials.TryGetValue(key, out var result))
                return result;
            throw new System.Exception("No credential");
        }
    }
}