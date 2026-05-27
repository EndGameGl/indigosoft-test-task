using IndigoSoft.QuotationCollector.Server.Models;

namespace IndigoSoft.QuotationCollector.Server.Services.Interfaces;

public interface IQuotationProcessorQueue
{
    Task ProcessQuotationAsync(Quotation quotation);
}
