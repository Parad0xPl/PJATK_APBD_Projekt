namespace FrontEnd.Utils;

public class SearchData
{
    public string Ticker { get; set; }
    public string Name { get; set; }
    public string PrimaryExchange { get; set; }

    public SearchData()
    {
        Ticker = "";
        Name = "";
        PrimaryExchange = "";
    }
    public SearchData(string Ticker, string Name, string PrimaryExchange)
    {
        this.Ticker = Ticker;
        this.Name = Name;
        this.PrimaryExchange = PrimaryExchange;
    }
    public override string ToString()
    {
        return $"{{ Ticker = {Ticker}, Name = {Name}, PrimaryExchange = {PrimaryExchange} }}";
    }
}