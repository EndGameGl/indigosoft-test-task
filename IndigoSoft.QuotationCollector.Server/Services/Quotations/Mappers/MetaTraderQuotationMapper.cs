using IndigoSoft.QuotationCollector.Server.Models;
using IndigoSoft.QuotationCollector.Server.Models.Quotations.Sources.Metatrader;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;

namespace IndigoSoft.QuotationCollector.Server.Services.Quotations.Mappers;

public class MetaTraderQuotationMapper : IQuotationMapper<MetaTraderQuotation>
{
    public Quotation MapFrom(MetaTraderQuotation sourceData)
    {
        return new Quotation()
        {
            Symbol = sourceData.Symbol,
            Ask = sourceData.Ask,
            Bid = sourceData.Bid,
            TimeStamp = DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(default),
        };
    }
}
