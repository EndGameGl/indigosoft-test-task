using System.Net.WebSockets;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;

namespace IndigoSoft.QuotationCollector.Server.Services.WebSockets;

public class WebSocketConnection : IWebSocketConnection
{
    private ClientWebSocket _client;

    public WebSocketState State => _client.State;

    public WebSocketConnection()
    {
        _client = new ClientWebSocket();
    }

    public void EnsureValidClient()
    {
        if (_client is { State: WebSocketState.Aborted or WebSocketState.Closed })
        {
            _client = new ClientWebSocket();
        }
    }

    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        await _client.ConnectAsync(uri, cancellationToken);
    }

    public async Task<WebSocketReceiveResult> ReceiveAsync(
        ArraySegment<byte> buffer,
        CancellationToken cancellationToken
    )
    {
        return await _client.ReceiveAsync(buffer, cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
