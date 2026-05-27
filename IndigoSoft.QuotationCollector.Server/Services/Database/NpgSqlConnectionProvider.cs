using System.Data.Common;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;
using Npgsql;

namespace IndigoSoft.QuotationCollector.Server.Services.Database;

public class NpgSqlConnectionProvider : IDbConnectionProvider
{
    private readonly IConfiguration _configuration;

    public NpgSqlConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbConnection GetConnection()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
    }
}
