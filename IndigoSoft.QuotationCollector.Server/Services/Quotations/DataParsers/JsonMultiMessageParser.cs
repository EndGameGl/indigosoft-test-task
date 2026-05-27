using System.Runtime.CompilerServices;
using System.Text.Json;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;

namespace IndigoSoft.QuotationCollector.Server.Services.Quotations.DataParsers;

public class JsonMultiMessageParser<TQuotationSourceType>
    : IQuotationDataParser<TQuotationSourceType>
    where TQuotationSourceType : class
{
    public async IAsyncEnumerable<TQuotationSourceType> ParseQuotationsAsync(
        Stream inputData,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        await foreach (
            var parsedQuotation in JsonSerializer.DeserializeAsyncEnumerable<TQuotationSourceType>(
                inputData,
                options: null,
                cancellationToken
            )
        )
        {
            yield return parsedQuotation;
        }
    }
}
