using IndigoSoft.QuotationCollector.Server.Models;
using IndigoSoft.QuotationCollector.Server.Models.Quotations.Sources.TestTradingApp;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;

namespace IndigoSoft.QuotationCollector.Server.Services.Quotations.Mappers;

public class TestTradingAppQuotationMapper : IQuotationMapper<TestTradingAppQuotation>
{
    public Quotation MapFrom(TestTradingAppQuotation sourceData)
    {
        return new Quotation()
        {
            Ask = sourceData.Ask,
            Bid = sourceData.Bid,
            Symbol = sourceData.Symbol,
            TimeStamp = DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(default),
        };
    }
}
