using System.Net.WebSockets;

namespace IndigoSoft.QuotationCollector.Server.Services.Interfaces;

public interface IWebSocketConnection : IDisposable
{
    WebSocketState State { get; }

    void EnsureValidClient();

    Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

    Task<WebSocketReceiveResult> ReceiveAsync(
        ArraySegment<byte> buffer,
        CancellationToken cancellationToken
    );
}
