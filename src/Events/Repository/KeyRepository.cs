using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Events
{
    public class KeyRepository
    {
        const string USERNAME_ALLOWED_CHARACTERS = @"^\w+$";
        private readonly NpgsqlConnectionFactory _factory;
        protected string TableName => $"public.keys";

        public KeyRepository(NpgsqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> Exists(string userName)
        {
            using (var connection = _factory.CreateConnection()) {
                return await connection.ExecuteScalarAsync<bool>($"SELECT true FROM public.keys WHERE name='{userName}'");
            }
        }

        public async Task Create(string username, string password)
        {
            using (var connection = _factory.CreateConnection())
            {
                var query = $"CREATE SCHEMA IF NOT EXISTS user_{username}";

                await connection.ExecuteAsync(query);

                query = $@"
                    CREATE TABLE IF NOT EXISTS user_{username}.events (
                        id int GENERATED ALWAYS AS IDENTITY primary key not null,
                        entity_id uuid NOT NULL,
                        network int NOT NULL references public.networks(id),
                        event_name text NOT NULL,
                        body bytea NOT NULL,
                        inserted_at timestamp(6) NOT NULL DEFAULT statement_timestamp()
                    );";
                await connection.ExecuteAsync(query);

                var parameters = new DynamicParameters();
                parameters.Add("username", username);
                parameters.Add("password", password);

                query = @"
                    INSERT INTO public.keys (name, password, key)
                    VALUES (@username, crypt(@password, gen_salt('bf', 8)), pgp_sym_encrypt(gen_salt('bf', 8), @password))
                ";  

                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<bool> ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (await TestPassword(userName, oldPassword))
            {
                var key = await GetKey(userName, oldPassword);
                using (var connection = _factory.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("newPassword", newPassword);
                    parameters.Add("key", key);
                    parameters.Add("name", userName);
                    await connection.ExecuteAsync($"UPDATE {TableName} SET password=crypt(@newPassword, gen_salt('bf')), key=pgp_sym_encrypt(@key, @newPassword)", parameters);
                    return true;
                }
            } else return false;
        }

        public async Task<string> GetKey(string userName, string password)
        {
            using (var connection = _factory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("userName", userName);
                parameters.Add("password", password);
                var result = await connection.ExecuteScalarAsync<string>($"SELECT pgp_sym_decrypt(key, @password) FROM {TableName} WHERE name = @userName", parameters);
                return result;
            }
        }

        private async Task<bool> TestPassword(string userName, string password)
        {
            using (var connection = _factory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("userName", userName);
                parameters.Add("password", password);
                return await connection.ExecuteScalarAsync<bool>($"SELECT (password = crypt(@password, password)) AS pswmatch FROM {TableName} WHERE name=@userName", parameters);
            }
        }
    }
}