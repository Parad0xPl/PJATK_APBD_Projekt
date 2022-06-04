namespace Shared.Entities;

public class Account
{
    public int ID { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;

    public virtual ICollection<ObservedStock> Watchlist { get; set; }

    public Account()
    {
        Watchlist = new HashSet<ObservedStock>();
    }
}