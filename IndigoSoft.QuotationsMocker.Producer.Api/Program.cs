using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using IndigoSoft.QuotationsMocker.Producer.Api.Assets;
using IndigoSoft.QuotationsMocker.Producer.Api.Endpoints;

await TestAssets.LoadTestQuotationDataAsync();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseWebSockets();

app.MapWebSocketTickerEndpoint(
    "/ws/metatrader",
    () =>
    {
        return new
        {
            symbol = "A",
            bid = (decimal)Math.Round(Random.Shared.NextDouble(), digits: 2),
        };
    }
);

app.MapWebSocketTickerEndpoint(
    "/ws/testtradingapp",
    () =>
    {
        return new
        {
            symbol = "A",
            bid = (decimal)Math.Round(Random.Shared.NextDouble(), digits: 2),
        };
    }
);

app.Run();
