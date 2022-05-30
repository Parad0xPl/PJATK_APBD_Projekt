namespace Server.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public int AccountID { get; set; }
    public virtual Account Account { get; set; }
}