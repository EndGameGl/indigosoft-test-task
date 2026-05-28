using FluentAssertions;
using IndigoSoft.QuotationCollector.Server.Models.Quotations.Sources.TestTradingApp;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;
using IndigoSoft.QuotationCollector.Server.Services.Quotations.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

namespace IndigoSoft.QuotationCollector.Tests.MapperTests;

public class TestTradingAppMapperFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
    {
        services.AddTransient<
            IQuotationMapper<TestTradingAppQuotation>,
            TestTradingAppQuotationMapper
        >();
    }

    protected override ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }
}

public class TestTradingAppMapperTests : TestBedWithDI<TestTradingAppMapperFixture>
{
    [Inject]
    private IQuotationMapper<TestTradingAppQuotation> _mapper { get; set; } = null!;

    public TestTradingAppMapperTests(
        ITestOutputHelper testOutputHelper,
        TestTradingAppMapperFixture fixture
    )
        : base(testOutputHelper, fixture) { }

    [Fact]
    public void Should_MapData()
    {
        // arrange
        var data = new TestTradingAppQuotation()
        {
            Symbol = "RTNPOWER.NS",
            Ask = 14.14m,
            Bid = 14.14m,
        };

        // act
        var mappedObject = _mapper.MapFrom(data);

        // assert
        mappedObject.Should().NotBeNull();

        mappedObject.Symbol.Should().BeEquivalentTo(data.Symbol);
        mappedObject.Ask.Should().Be(data.Ask);
        mappedObject.Bid.Should().Be(data.Bid);
    }
}
