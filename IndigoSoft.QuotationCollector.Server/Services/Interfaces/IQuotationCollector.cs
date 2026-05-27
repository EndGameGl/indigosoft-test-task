namespace IndigoSoft.QuotationCollector.Server.Services.Interfaces;

public interface IQuotationCollector
{
    Task StartCollectionAsync(CancellationToken cancellationToken);
}

public interface IQuotationCollector<TSourceQuotation> : IQuotationCollector;
