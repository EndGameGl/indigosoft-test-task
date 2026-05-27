using IndigoSoft.QuotationCollector.Server.Configuration.QuotationProcessors;

namespace IndigoSoft.QuotationCollector.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static QuotationProcessorBuilder<TSourceQuotation> AddQuotationProcessor<TSourceQuotation>(
        this IServiceCollection services
    )
    {
        return new QuotationProcessorBuilder<TSourceQuotation>(services);
    }
}
