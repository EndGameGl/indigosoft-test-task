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
        return TestAssets
            .TestQuotations.Select(x =>
            {
                return new
                {
                    symbol = x.StockTicker,
                    bid = Math.Round(x.Bid.GetValueOrDefault(), decimals: 2),
                    ask = Math.Round(x.Ask.GetValueOrDefault(), decimals: 2),
                };
            })
            .OrderBy(x => Random.Shared.Next())
            .First();
    }
);

app.MapWebSocketTickerEndpoint(
    "/ws/testtradingapp",
    () =>
    {
        return TestAssets
            .TestQuotations.Select(x =>
            {
                return new
                {
                    s = x.StockTicker,
                    b = Math.Round(x.Bid.GetValueOrDefault(), decimals: 2),
                    a = Math.Round(x.Ask.GetValueOrDefault(), decimals: 2),
                };
            })
            .OrderBy(x => Random.Shared.Next())
            .Take(Random.Shared.Next(10, 30));
    }
);

app.Run();
