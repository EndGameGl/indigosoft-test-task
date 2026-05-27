using IndigoSoft.QuotationCollector.Server.Services.Interfaces;

namespace IndigoSoft.QuotationCollector.Server.Services.Hosted;

public class BackgroundCollectorStarter : BackgroundService
{
    private readonly IQuotationCollector[] _quotationCollectors;

    public BackgroundCollectorStarter(IEnumerable<IQuotationCollector> quotationCollectors)
    {
        _quotationCollectors = [.. quotationCollectors];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var quotationCollector in _quotationCollectors)
        {
            await quotationCollector.StartCollectionAsync(stoppingToken);
        }
    }
}
