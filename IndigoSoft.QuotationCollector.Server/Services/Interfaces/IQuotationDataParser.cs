namespace IndigoSoft.QuotationCollector.Server.Services.Interfaces;

public interface IQuotationDataParser<TSourceQuotation>
{
    IAsyncEnumerable<TSourceQuotation> ParseQuotationsAsync(
        Stream inputData,
        CancellationToken cancellationToken
    );
}
