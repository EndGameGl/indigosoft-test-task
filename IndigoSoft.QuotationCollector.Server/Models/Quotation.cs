namespace IndigoSoft.QuotationCollector.Server.Models;

public class Quotation
{
    public string Symbol { get; set; }

    public decimal Bid { get; set; }

    public decimal Ask { get; set; }

    public DateTime TimeStamp { get; set; }
}
