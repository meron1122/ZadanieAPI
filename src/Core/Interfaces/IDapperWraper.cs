using System.Data;

namespace Core.Interfaces
{
    public interface IDapperWrapper
    {
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? parameters = null);
        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object? parameters = null);
        Task<int> ExecuteAsync(IDbConnection connection, string sql, object? parameters = null);
    }
}
