using System.Text.Json;
using FluentAssertions;
using IndigoSoft.QuotationCollector.Server.Models.Quotations.Sources.Metatrader;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;
using IndigoSoft.QuotationCollector.Server.Services.Quotations.DataParsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

namespace IndigoSoft.QuotationCollector.Tests.ParserTests;

public class JsonSingleMessageParserFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
    {
        services.AddTransient<
            IQuotationDataParser<MetaTraderQuotation>,
            JsonSingleMessageParser<MetaTraderQuotation>
        >();
    }

    protected override ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }
}

public class JsonSingleMessageParserTests : TestBedWithDI<JsonSingleMessageParserFixture>
{
    [Inject]
    private IQuotationDataParser<MetaTraderQuotation> Parser { get; set; }

    public JsonSingleMessageParserTests(
        ITestOutputHelper testOutputHelper,
        JsonSingleMessageParserFixture fixture
    )
        : base(testOutputHelper, fixture) { }

    private const string TestMessage = """
        {
            "symbol": "",
            "bid": 0,
            "ask": 0
        }
        """;

    private const string InvalidTestMessage = """
        [{
            "symbol": "",
            "bid": 0,
            "ask": 0
        }]
        """;

    [Fact]
    public async Task Should_ParseMessage()
    {
        // arrange
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        writer.Write(TestMessage);
        writer.Flush();
        memoryStream.Position = 0;

        //act
        Func<Task<MetaTraderQuotation[]>> act = Parser.Awaiting(x =>
            x.ParseQuotationsAsync(memoryStream, CancellationToken.None)
                .ToArrayAsync(CancellationToken.None)
        );

        // assert
        (await act.Should().NotThrowAsync())
            .Which.Should()
            .NotBeEmpty()
            .And.ContainSingle();
    }

    [Fact]
    public async Task Should_ThrowParseException()
    {
        // arrange
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        writer.Write(InvalidTestMessage);
        writer.Flush();
        memoryStream.Position = 0;

        //act

        Func<Task<MetaTraderQuotation[]>> act = Parser.Awaiting(x =>
            x.ParseQuotationsAsync(memoryStream, CancellationToken.None)
                .ToArrayAsync(CancellationToken.None)
        );

        // assert
        await act.Should().ThrowAsync<JsonException>();
    }
}
