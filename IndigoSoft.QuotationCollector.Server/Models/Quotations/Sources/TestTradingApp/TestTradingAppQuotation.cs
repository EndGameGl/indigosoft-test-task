using System.Text.Json.Serialization;

namespace IndigoSoft.QuotationCollector.Server.Models.Quotations.Sources.TestTradingApp;

public class TestTradingAppQuotation
{
    /// <summary>
    ///     Symbol name
    /// </summary>
    [JsonPropertyName("s")]
    public string Symbol { get; set; }

    /// <summary>
    ///     Current bid price
    /// </summary>
    [JsonPropertyName("b")]
    public decimal Bid { get; set; }

    /// <summary>
    ///     Current ask price
    /// </summary>
    [JsonPropertyName("a")]
    public decimal Ask { get; set; }
}
