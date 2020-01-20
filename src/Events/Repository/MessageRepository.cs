using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Events
{
    public class MessageRepository : NpgsqlExternalEntityRepository<Message>
    {
        public MessageRepository(string connectionString, string schema) : base(connectionString, schema)
        {
        }

        public async Task Add(Message message)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("Id", message.Id);
                parameters.Add("SenderId", message.SenderId);
                parameters.Add("BodyPlain", message.BodyPlain);
                parameters.Add("BodyParsed", message.BodyParsed);
                parameters.Add("CreatedAt", message.CreatedAt);
                parameters.Add("Network", message.Network);

                await connection.ExecuteAsync($@"
                INSERT INTO {TableName} 
                    (id, sender_id, body_plan, body_parsed, created_at, network, external_id) 
                VALUES
                    (@Id, @SenderId, @BodyPlain, @BodyParsed, @CreatedAt, @Network, @ExternalId)", 
                parameters);
            }
        }
    }
}