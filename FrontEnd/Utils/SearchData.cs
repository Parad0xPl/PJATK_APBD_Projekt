namespace FrontEnd.Utils;

public class SearchData
{
    public string? Ticker { get; set; }
    public string? Name { get; set; }
    public string? PrimaryExchange { get; set; }

    public SearchData()
    {
        Ticker = "";
        Name = "";
        PrimaryExchange = "";
    }
    public SearchData(string? ticker, string? name, string? primaryExchange)
    {
        this.Ticker = ticker;
        this.Name = name;
        this.PrimaryExchange = primaryExchange;
    }
    public override string ToString()
    {
        return $"{{ Ticker = {Ticker}, Name = {Name}, PrimaryExchange = {PrimaryExchange} }}";
    }
}