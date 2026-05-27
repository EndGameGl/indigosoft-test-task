using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace IndigoSoft.QuotationsMocker.Producer.Api.Endpoints;

public static class WebSocketEndpoints
{
    public static void MapWebSocketTickerEndpoint(
        this WebApplication webApplication,
        string pattern,
        Func<object> createData
    )
    {
        webApplication.Map(
            pattern,
            async ctx =>
            {
                if (ctx.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await ctx.WebSockets.AcceptWebSocketAsync();
                    while (!ctx.RequestAborted.IsCancellationRequested)
                    {
                        var data = JsonSerializer.Serialize(createData());
                        var bytes = Encoding.UTF8.GetBytes(data);
                        await webSocket.SendAsync(
                            bytes,
                            WebSocketMessageType.Text,
                            endOfMessage: true,
                            ctx.RequestAborted
                        );
                        await Task.Delay(1000 / 200);
                    }
                }
                else
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
        );
    }
}
