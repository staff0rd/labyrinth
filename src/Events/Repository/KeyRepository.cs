using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Events
{
    public class KeyRepository
    {
        protected readonly string _connectionString;
        protected string TableName => $"public.keys";

        public KeyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (await TestPassword(userName, oldPassword))
            {
                var key = GetKey(userName, oldPassword);
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("newPassword", newPassword);
                    parameters.Add("key", key);
                    parameters.Add("name", userName);
                    await connection.ExecuteAsync($"UPDATE {TableName} SET password=crypt(@newPassword, gen_salt('bf')), key=pgp_sym_encrypt(@key, @newPassword)", parameters);
                }
            } 
        }

        public async Task<string> GetKey(string userName, string password)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("userName", userName);
                parameters.Add("password", password);
                return await connection.ExecuteScalarAsync<string>($"SELECT pgp_sym_decrypt(key, @password) FROM {TableName} WHERE name = @userName", parameters);
            }
        }

        private async Task<bool> TestPassword(string userName, string password)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("userName", userName);
                parameters.Add("password", password);
                return await connection.ExecuteScalarAsync<bool>($"SELECT (password = crypt(@password, password)) AS pswmatch FROM {TableName} WHERE name=@userName", parameters);
            }
        }
    }
}