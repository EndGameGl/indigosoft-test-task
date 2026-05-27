using System.Diagnostics;
using System.Text.Json;
using IndigoSoft.QuotationCollector.Server.Models;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;
using Prometheus;
using StackExchange.Redis;

namespace IndigoSoft.QuotationCollector.Server.Services;

public class QuotationProcessorQueue : IQuotationProcessorQueue
{
    private static readonly TimeSpan QuotationTtl = TimeSpan.FromHours(5);

    private readonly ILogger<QuotationProcessorQueue> _logger;
    private readonly IQuotationRepository _quotationRepository;

    private readonly ConnectionMultiplexer _multiplexer;

    private static readonly Counter QuotationsProcessedCounter = Metrics.CreateCounter(
        "quotation_processed",
        "A number of quotations processed overall."
    );
    private static readonly Counter QuotationsDuplicatesCounter = Metrics.CreateCounter(
        "quotation_duplicates_found",
        "A number of quotations duplicates found."
    );

    public QuotationProcessorQueue(
        ILogger<QuotationProcessorQueue> logger,
        IQuotationRepository quotationRepository
    )
    {
        _logger = logger;
        _quotationRepository = quotationRepository;
        _multiplexer = ConnectionMultiplexer.Connect(
            new ConfigurationOptions()
            {
                EndPoints = { { "localhost", 6379 } },
                Password = "sh2TajXeytyAG1eHYPit",
                ReconnectRetryPolicy = new ExponentialRetry(500, 2000),
                ConnectRetry = 100,
                AbortOnConnectFail = false,
            }
        );
    }

    public async Task ProcessQuotationAsync(Quotation quotation)
    {
        var timeStart = Stopwatch.GetTimestamp();
        try
        {
            var database = _multiplexer.GetDatabase();
            var key = $"{quotation.Symbol}:{quotation.Ask}:{quotation.Bid}:{quotation.TimeStamp}";

            // key is already in database, ignoring
            if (await database.KeyExistsAsync(key))
            {
                _logger.LogInformation("Duplicate quotation, ignoring data");
                QuotationsDuplicatesCounter.Inc();
                return;
            }

            // other processor may have already added quotation with same parameters after checking key existence
            if (
                !await database.StringSetAsync(
                    key,
                    JsonSerializer.Serialize(quotation),
                    expiry: QuotationTtl,
                    When.NotExists
                )
            )
            {
                _logger.LogInformation("Quotation already added, shouldn't process further");
                QuotationsDuplicatesCounter.Inc();
                return;
            }

            await OnQuotationProcessedAsync(quotation);

            _logger.LogInformation("Quotation received");
        }
        finally
        {
            var timeSpent = Stopwatch.GetElapsedTime(timeStart);
            _logger.LogInformation("Processed quotation in {TimeSpent}", timeSpent);
            QuotationsProcessedCounter.Inc();
        }
    }

    private async Task OnQuotationProcessedAsync(Quotation quotation)
    {
        await _quotationRepository.InsertQuotationAsync(quotation);
    }
}
