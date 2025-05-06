using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiHost
{

    public static class DbHelper
    {
        private static string? _connectionString;

        // Call this method during app startup to set the connection string
        public static void Initialize(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private static IDbConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Database connection string has not been initialized.");

            return new SqlConnection(_connectionString);
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(sql, parameters);
        }

        public static async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, parameters);
        }

        public static async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procedureName, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public static async Task<int> ExecuteStoredProcedureAsync(string procedureName, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                // Execute the query asynchronously and get the first result (or default if none)
                return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            }
        }
        public static async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<T>(sql, parameters);
        }

    }

}
