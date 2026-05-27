using System.Runtime.CompilerServices;
using System.Text.Json;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;

namespace IndigoSoft.QuotationCollector.Server.Services.Quotations.DataParsers;

public class JsonSingleMessageParser<TQuotationSourceType>
    : IQuotationDataParser<TQuotationSourceType>
    where TQuotationSourceType : class
{
    public async IAsyncEnumerable<TQuotationSourceType> ParseQuotationsAsync(
        Stream inputData,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        yield return await JsonSerializer.DeserializeAsync<TQuotationSourceType>(
            inputData,
            options: null,
            cancellationToken
        );
    }
}
