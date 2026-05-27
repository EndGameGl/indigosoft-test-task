using IndigoSoft.QuotationCollector.Server.Models;

namespace IndigoSoft.QuotationCollector.Server.Services.Interfaces;

public interface IQuotationRepository
{
    Task InsertQuotationAsync(Quotation quotation);
}
