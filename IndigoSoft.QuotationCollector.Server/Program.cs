using IndigoSoft.QuotationCollector.Server.Extensions;
using IndigoSoft.QuotationCollector.Server.Models.Quotations.Sources.Metatrader;
using IndigoSoft.QuotationCollector.Server.Models.Quotations.Sources.TestTradingApp;
using IndigoSoft.QuotationCollector.Server.Services;
using IndigoSoft.QuotationCollector.Server.Services.Database;
using IndigoSoft.QuotationCollector.Server.Services.Hosted;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;
using IndigoSoft.QuotationCollector.Server.Services.Quotations.DataParsers;
using IndigoSoft.QuotationCollector.Server.Services.Quotations.Mappers;
using IndigoSoft.QuotationCollector.Server.Services.WebSockets;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IDbConnectionProvider, NpgSqlConnectionProvider>();
builder.Services.AddSingleton<IQuotationRepository, QuotationRepository>();

builder.Services.AddTransient<IWebSocketConnection, WebSocketConnection>();
builder.Services.AddSingleton<IQuotationProcessorQueue, QuotationProcessorQueue>();

builder
    .Services.AddQuotationProcessor<MetaTraderQuotation>()
    .WithParser<JsonSingleMessageParser<MetaTraderQuotation>>()
    .WithMapper<MetaTraderQuotationMapper>()
    .Build(o =>
    {
        o.Host = builder
            .Configuration.GetRequiredSection("DataBrokerSettings:MetaTrader")
            .GetValue<string>("Host")!;
    });

builder
    .Services.AddQuotationProcessor<TestTradingAppQuotation>()
    .WithParser<JsonMultiMessageParser<TestTradingAppQuotation>>()
    .WithMapper<TestTradingAppQuotationMapper>()
    .Build(o =>
    {
        o.Host = builder
            .Configuration.GetRequiredSection("DataBrokerSettings:TestTradingApp")
            .GetValue<string>("Host")!;
    });

builder.Services.AddHostedService<BackgroundCollectorStarter>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();
app.UseHttpMetrics();

app.MapMetrics("/metrics");

app.Run();
