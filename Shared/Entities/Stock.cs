namespace Server.Entities;

public class Stock
{
    public int Id { get; set; }
    public string Ticker { get; set; }
    public string RequestJSON { get; set; }
    public DateTime UpdateTime { get; set; }
    
    public virtual ICollection<ObservedStock> Observers { get; set; }

    public Stock()
    {
        Observers = new HashSet<ObservedStock>();
    }
}