using System;
using Microsoft.Graph;

namespace Events
{
    public class User : IExternalEntity
    {
        public static string UnknownUserId = new Guid("00000000-abd2-4d5a-8d6b-4d9cb28c9c07").ToString();
        public string Id { get; set;}
        public string AvatarUrl { get; set; }
        public string Name { get; set;}
        public DateTimeOffset KnownSince { get; set;}
        public Network Network { get; set;}
        public string Description { get; set; }
        public Guid SourceId { get; set; }

        public static User From(Identity user, Guid sourceId)
        {
            if (user == null)
            {
                return new User {
                    Id = UnknownUserId,
                    Name = "Unknown",
                    SourceId = sourceId,
                    Network = Network.Teams,
                };
            }
            return new User {
                Id = user.Id,
                Name = user.DisplayName,
                SourceId = sourceId,
                Network = Network.Teams,
            };
        }
        
        public static User From(Rest.Yammer.User user, Guid sourceId)
        {
            return new User
            {
                AvatarUrl = user.MugshotUrl.AbsoluteUri,
                Description = user.JobTitle,
                Name = user.FullName,
                Network = Network.Yammer,
                Id = user.Id.AsId<User>(Network.Yammer),
                SourceId = sourceId
            };
        }
    }
    public static class IdExtension {
        public static string AsId<T>(this object externalId, Network network) {
            var typeName = typeof(T).Name;
            var id = $"{network.ToString().ToLower()}/{typeName.ToLower()}/{externalId}";
            return id;
        }
    }
}