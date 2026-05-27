using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using IndigoSoft.QuotationsMocker.Producer.Api.Assets;

await TestAssets.LoadTestQuotationDataAsync();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseWebSockets();

app.Map(
    "/ws/metatrader",
    async ctx =>
    {
        if (ctx.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await ctx.WebSockets.AcceptWebSocketAsync();
            while (!ctx.RequestAborted.IsCancellationRequested)
            {
                var data = JsonSerializer.Serialize(
                    new
                    {
                        symbol = "A",
                        bid = (decimal)Math.Round(Random.Shared.NextDouble(), digits: 2),
                    }
                );
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

app.Run();
