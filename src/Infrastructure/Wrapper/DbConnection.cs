using Core.Interfaces;
using Npgsql;
using System.Data;


namespace Infrastructure.Wrapper
{
    public class DbConnection : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
