namespace Shared.Entities;

public class Stock
{
    public int Id { get; set; }
    public string Ticker { get; set; } = null!;
    public string RequestJson { get; set; } = null!;
    public DateTime UpdateTime { get; set; }
    
    public virtual ICollection<ObservedStock> Observers { get; set; }

    public Stock()
    {
        Observers = new HashSet<ObservedStock>();
    }
}