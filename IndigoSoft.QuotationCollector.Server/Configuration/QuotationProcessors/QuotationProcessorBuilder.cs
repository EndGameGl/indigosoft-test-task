using IndigoSoft.QuotationCollector.Server.Services;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;

namespace IndigoSoft.QuotationCollector.Server.Configuration.QuotationProcessors;

public class QuotationProcessorBuilder<TSourceQuotation>
{
    private readonly IServiceCollection _services;

    public QuotationProcessorBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public QuotationProcessorBuilder<TSourceQuotation> WithParser<TQuotationParser>()
        where TQuotationParser : class, IQuotationDataParser<TSourceQuotation>
    {
        _services.AddSingleton<IQuotationDataParser<TSourceQuotation>, TQuotationParser>();
        return this;
    }

    public QuotationProcessorBuilder<TSourceQuotation> WithMapper<TQuotationMapper>()
        where TQuotationMapper : class, IQuotationMapper<TSourceQuotation>
    {
        _services.AddSingleton<IQuotationMapper<TSourceQuotation>, TQuotationMapper>();
        return this;
    }

    public IServiceCollection Build(
        Action<QuotationCollectorOptions<QuotationCollector<TSourceQuotation>>>? configureOptions =
            null
    )
    {
        _services.Configure(configureOptions);
        _services.AddSingleton<IQuotationCollector, QuotationCollector<TSourceQuotation>>();
        return _services;
    }
}
