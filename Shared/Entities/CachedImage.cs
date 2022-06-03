namespace Server.Entities;

public class CachedImage
{
    public string Url { get; set; }
    public byte[] Data { get; set; }
    public string Type { get; set; }
    public DateTime CreationDate { get; set; }
}