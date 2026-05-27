using Dapper;
using IndigoSoft.QuotationCollector.Server.Models;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;

namespace IndigoSoft.QuotationCollector.Server.Services.Database;

public class QuotationRepository : IQuotationRepository
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    private readonly ILogger<QuotationRepository> _logger;

    public QuotationRepository(
        IDbConnectionProvider dbConnectionProvider,
        ILogger<QuotationRepository> logger
    )
    {
        _dbConnectionProvider = dbConnectionProvider;
        _logger = logger;
    }

    private const string InsertQuotationQuery = """
        INSERT INTO public."Quotations" (symbol, ask, bid, timestamp) 
        VALUES (@Symbol, @Ask, @Bid, @Timestamp);
        """;

    public async Task InsertQuotationAsync(Quotation quotation)
    {
        try
        {
            await using var connection = _dbConnectionProvider.GetConnection();

            await connection.ExecuteAsync(
                InsertQuotationQuery,
                new
                {
                    Symbol = quotation.Symbol,
                    Ask = quotation.Ask,
                    Bid = quotation.Bid,
                    Timestamp = quotation.TimeStamp,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encountered error when inserting new quotation into database");
        }
    }
}
