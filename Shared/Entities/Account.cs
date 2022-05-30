namespace Server.Entities;

public class Account
{
    public int ID { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }

    public virtual ICollection<ObservedStock> Watchlist { get; set; }

    public Account()
    {
        Watchlist = new HashSet<ObservedStock>();
    }
}