using FluentAssertions;
using IndigoSoft.QuotationCollector.Server.Services.Database;
using IndigoSoft.QuotationCollector.Server.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

namespace IndigoSoft.QuotationCollector.Tests.DatabaseTests;

public class QuotationDatabaseFixture : TestBedFixture, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder(
        "postgres:latest"
    ).Build();

    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        configuration["ConnectionStrings:Postgres"] = _postgresContainer.GetConnectionString();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();

        services.AddSingleton<IDbConnectionProvider, NpgSqlConnectionProvider>();
        services.AddSingleton<IQuotationRepository, QuotationRepository>();
    }

    public async ValueTask InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        await _postgresContainer.ExecScriptAsync(
            """
            CREATE TABLE "Quotations"
            (
                symbol character varying(16) NOT NULL,
                ask numeric NOT NULL,
                bid numeric NOT NULL,
                "timestamp" timestamp without time zone NOT NULL
            )
            """
        );
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await _postgresContainer.DisposeAsync();
    }
}

[TestCaseOrderer(typeof(Xunit.Microsoft.DependencyInjection.TestsOrder.TestPriorityOrderer))]
public class QuotationDatabaseTests : TestBedWithDI<QuotationDatabaseFixture>
{
    [Inject]
    private IQuotationRepository Repository { get; set; } = null!;

    public QuotationDatabaseTests(
        ITestOutputHelper testOutputHelper,
        QuotationDatabaseFixture fixture
    )
        : base(testOutputHelper, fixture) { }

    [Fact]
    [TestOrder(1)]
    public async Task Should_NotInsertNullSymbol()
    {
        await Repository.InsertQuotationAsync(
            new Server.Models.Quotation() { Symbol = null, TimeStamp = DateTime.UtcNow }
        );

        using var provider = GetService<IDbConnectionProvider>()!.GetConnection();

        await provider.OpenAsync(TestContext.Current.CancellationToken);

        using var command = provider.CreateCommand();

        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = """
            SELECT * FROM "Quotations"
            """;

        using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);

        reader.HasRows.Should().BeFalse();
    }

    [Fact]
    [TestOrder(2)]
    public async Task Should_InsertValue()
    {
        await Repository.InsertQuotationAsync(
            new Server.Models.Quotation() { Symbol = "A", TimeStamp = DateTime.UtcNow }
        );

        using var provider = GetService<IDbConnectionProvider>()!.GetConnection();

        await provider.OpenAsync(TestContext.Current.CancellationToken);

        using var command = provider.CreateCommand();

        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = """
            SELECT * FROM "Quotations"
            """;

        using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);

        reader.HasRows.Should().BeTrue();
    }
}
