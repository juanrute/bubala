using System.Data;

namespace Bubala.Application.Database;

public interface IDbConectionFactory
{
    Task<IDbConnection> CreateConectionAsync();
}