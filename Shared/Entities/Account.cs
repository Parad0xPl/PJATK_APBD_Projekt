namespace Server.Entities;

public class Account
{
    public int ID { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
}