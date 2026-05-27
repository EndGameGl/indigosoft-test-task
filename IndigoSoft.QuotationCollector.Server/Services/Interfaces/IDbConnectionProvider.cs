using System.Data.Common;

namespace IndigoSoft.QuotationCollector.Server.Services.Interfaces;

public interface IDbConnectionProvider
{
    DbConnection GetConnection();
}
