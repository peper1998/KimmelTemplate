using Microsoft.Data.SqlClient;
using System.Data;

namespace KimmelTemplate.Infrastructure.Factories
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);
    }
}
