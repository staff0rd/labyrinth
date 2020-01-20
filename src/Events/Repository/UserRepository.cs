using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Events
{
    public class UserRepository : NpgsqlExternalEntityRepository<User>
    {
        public UserRepository(string connectionString, string schema) : base(connectionString, schema)
        {
        }

        public async Task Add(User user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                
                parameters.Add("AvatarUrl", user.AvatarUrl);
                parameters.Add("Description", user.Description);
                parameters.Add("Name", user.Name);
                parameters.Add("Network", user.Network);
                parameters.Add("Id", user.Id);

                await connection.ExecuteAsync($@"
                INSERT INTO {TableName} 
                    (id, network, external_id, avatar_url, name, known_since, description) 
                VALUES
                    (@Id, @Network, @ExternalId, @AvatarUrl, @Name, null, @Description)", 
                parameters);
            }
        }
    }
}