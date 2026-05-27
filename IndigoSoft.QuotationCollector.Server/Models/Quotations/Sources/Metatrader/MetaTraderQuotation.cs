using System.Text.Json.Serialization;

namespace IndigoSoft.QuotationCollector.Server.Models.Quotations.Sources.Metatrader;

public class MetaTraderQuotation
{
    /// <summary>
    ///     Symbol name
    /// </summary>
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    /// <summary>
    ///     Symbol description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    ///     Current bid price
    /// </summary>
    [JsonPropertyName("bid")]
    public decimal Bid { get; set; }

    /// <summary>
    ///     Current ask price
    /// </summary>
    [JsonPropertyName("ask")]
    public decimal Ask { get; set; }

    /// <summary>
    ///     Spread in points
    /// </summary>
    [JsonPropertyName("spread")]
    public int Spread { get; set; }

    /// <summary>
    ///     Number of decimal places
    /// </summary>
    [JsonPropertyName("digits")]
    public int Digits { get; set; }

    [JsonPropertyName("trade_contract_size")]
    public decimal TradeContractSize { get; set; }

    [JsonPropertyName("min_volume")]
    public decimal MinVolume { get; set; }

    [JsonPropertyName("max_volume")]
    public decimal MaxVolume { get; set; }

    [JsonPropertyName("volume_step")]
    public decimal VolumeStep { get; set; }
}
