using System.Data;
using Npgsql;

namespace Bubala.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConectionAsync();
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    public readonly string _connectionString;
    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}