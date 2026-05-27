using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace IndigoSoft.QuotationsMocker.Producer.Api.Assets;

public static class TestAssets
{
    public static TestQuotationData[] TestQuotations { get; private set; }

    public static async Task LoadTestQuotationDataAsync()
    {
        using var reader = new StreamReader("Assets\\TestDataSet.csv");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var csv = new CsvReader(reader, config);
        TestQuotations = await csv.GetRecordsAsync<TestQuotationData>().ToArrayAsync();
    }
}

public class TestQuotationData
{
    [Name("stock_ticker")]
    public string StockTicker { get; set; }

    [Name("open")]
    public decimal Open { get; set; }

    [Name("bid")]
    public decimal? Bid { get; set; }

    [Name("ask")]
    public decimal? Ask { get; set; }

    [Name("volume")]
    public int Volume { get; set; }
}
