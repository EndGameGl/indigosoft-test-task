using System.Buffers;
using System.Net.WebSockets;
using IndigoSoft.QuotationCollector.Server.Configuration;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace IndigoSoft.QuotationCollector.Server.Services;

public class QuotationCollector<TSourceQuotation>
    : IQuotationCollector<TSourceQuotation>,
        IDisposable
{
    private const int ReceiveBufferSize = 8192;
    private Task? _receiveLoop;

    private readonly ILogger<QuotationCollector<TSourceQuotation>> _logger;
    private readonly IQuotationDataParser<TSourceQuotation> _quotationDataParser;
    private readonly IQuotationMapper<TSourceQuotation> _mapper;
    private readonly IOptions<
        QuotationCollectorOptions<QuotationCollector<TSourceQuotation>>
    > _options;
    private readonly IWebSocketConnection _webSocketConnection;
    private readonly IQuotationProcessorQueue _quotationProcessorQueue;

    public QuotationCollector(
        ILogger<QuotationCollector<TSourceQuotation>> logger,
        IQuotationProcessorQueue quotationProcessorQueue,
        IQuotationDataParser<TSourceQuotation> quotationDataParser,
        IQuotationMapper<TSourceQuotation> mapper,
        IOptions<QuotationCollectorOptions<QuotationCollector<TSourceQuotation>>> options,
        IWebSocketConnection webSocketConnection
    )
    {
        _logger = logger;
        _quotationProcessorQueue = quotationProcessorQueue;
        _quotationDataParser = quotationDataParser;
        _mapper = mapper;
        _options = options;
        _webSocketConnection = webSocketConnection;
    }

    public async Task StartCollectionAsync(CancellationToken cancellationToken)
    {
        _receiveLoop = HandleReceiveLoopInternalAsync(cancellationToken);
    }

    private async Task HandleReceiveLoopInternalAsync(CancellationToken cancellationToken)
    {
        await ConnectWsInternalAsync(cancellationToken);

        LoopStart:
        MemoryStream? outputStream = null;
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);
                try
                {
                    outputStream = new MemoryStream(ReceiveBufferSize);
                    WebSocketReceiveResult receiveResult;
                    do
                    {
                        receiveResult = await _webSocketConnection!.ReceiveAsync(
                            buffer,
                            cancellationToken
                        );

                        if (receiveResult.MessageType != WebSocketMessageType.Close)
                            outputStream.Write(buffer, 0, receiveResult.Count);
                    } while (!receiveResult.EndOfMessage);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogInformation("Server connection closed.");
                        await ConnectWsInternalAsync(cancellationToken);
                    }

                    outputStream.Position = 0;
                    await HandleDataReceivedAsync(outputStream, cancellationToken);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
        catch (TaskCanceledException) { }
        catch (WebSocketException)
        {
            await ConnectWsInternalAsync(cancellationToken);
            goto LoopStart;
        }
        finally
        {
            if (outputStream is not null)
                await outputStream.DisposeAsync();
        }
    }

    private async Task ConnectWsInternalAsync(CancellationToken cancellationToken)
    {
        var delay = TimeSpan.FromSeconds(1);
        var maxDelay = TimeSpan.FromSeconds(30);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _webSocketConnection.EnsureValidClient();

                _logger.LogInformation("Connecting to server...");

                await _webSocketConnection.ConnectAsync(
                    new Uri(_options.Value.Host),
                    cancellationToken
                );
                return;
            }
            catch (WebSocketException) { }
            catch (OperationCanceledException)
            {
                break;
            }

            _logger.LogInformation("Failed to connect, retrying in {Delay}", delay);
            await Task.Delay(delay, cancellationToken);
            delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, maxDelay.TotalSeconds));
        }

        _logger.LogInformation("Connection established!");
    }

    private async Task HandleDataReceivedAsync(
        Stream resultData,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await foreach (
                var sourceQuotation in _quotationDataParser.ParseQuotationsAsync(
                    resultData,
                    cancellationToken
                )
            )
            {
                var quotation = _mapper.MapFrom(sourceQuotation);
                await _quotationProcessorQueue.ProcessQuotationAsync(quotation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle received quotation data.");
        }
    }

    public void Dispose()
    {
        _receiveLoop?.Dispose();
        _webSocketConnection?.Dispose();
    }
}
