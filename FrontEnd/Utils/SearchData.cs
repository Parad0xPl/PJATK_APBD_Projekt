namespace FrontEnd.Utils;

public record SearchData(string Ticker, string Name, string PrimaryExchange)
{
    public override string ToString()
    {
        return $"{{ Ticker = {Ticker}, Name = {Name}, PrimaryExchange = {PrimaryExchange} }}";
    }
}