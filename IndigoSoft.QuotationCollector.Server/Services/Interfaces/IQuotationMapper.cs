using IndigoSoft.QuotationCollector.Server.Models;

namespace IndigoSoft.QuotationCollector.Server.Services.Interfaces;

public interface IQuotationMapper<TSource>
{
    Quotation MapFrom(TSource sourceData);
}
