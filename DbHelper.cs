using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.Data;

namespace ApiHost
{

    public static class DbHelper
    {
        private static string? _connectionString;
        private static string? _provider;

        // Call this method during app startup to set the connection string
        public static void Initialize(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _provider = configuration.GetValue<string>("DatabaseProvider");
        }

        private static IDbConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(_connectionString) || string.IsNullOrEmpty(_provider))
                throw new InvalidOperationException("Database config not initialized.");

            return _provider.ToLower() switch
            {
                "postgres" => new NpgsqlConnection(_connectionString),
                "sqlserver" => new SqlConnection(_connectionString),
                _ => throw new NotSupportedException($"Unsupported provider: {_provider}")
            };
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


        public static async Task<int> CallPostgresProcedureAsync(string procName, object parameters)
        {
            using var connection = CreateConnection();

            // Extract property names to build param list: @p_name, @p_description, ...
            var props = parameters.GetType().GetProperties();
            var paramNames = string.Join(", ", props.Select(p => "@" + p.Name));

            // Build CALL statement: CALL proc_name(@p_name, @p_description, ...)
            var sql = $"CALL {procName}({paramNames});";

            return await connection.ExecuteAsync(sql, parameters);
        }

        public static async Task<int> ExecuteStoredProcedureAsync(string procedureName, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public static async Task<IEnumerable<T>> QueryPostgresFunctionAsync<T>(string functionName, object? parameters = null)
        {
            using var connection = CreateConnection();

            // Build function call syntax with parameter placeholders
            var paramNames = parameters?.GetType().GetProperties().Select(p => "@" + p.Name).ToArray();
            var paramList = paramNames != null ? string.Join(", ", paramNames) : "";
            var sql = $"SELECT * FROM {functionName}({paramList});";

            return await connection.QueryAsync<T>(sql, parameters);
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
