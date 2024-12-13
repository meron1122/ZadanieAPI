using Core.Interfaces;
using Dapper;
using System.Data;


namespace Infrastructure.Wrapper
{
    public class DapperWrapper : IDapperWrapper
    {
        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? parameters = null)
        {
            return await connection.QueryAsync<T>(sql, parameters);
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object? parameters = null)
        {
            return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
        }

        public async Task<int> ExecuteAsync(IDbConnection connection, string sql, object? parameters = null)
        {
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
