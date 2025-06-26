using System.Data;
using Npgsql;

namespace Bubala.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConectionAsync(CancellationToken cancellationToken = default);
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    public readonly string _connectionString;
    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}