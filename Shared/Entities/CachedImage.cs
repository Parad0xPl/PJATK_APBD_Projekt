namespace Shared.Entities;

public class CachedImage
{
    public string Url { get; set; } = null!;
    public byte[] Data { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime CreationDate { get; set; }
}