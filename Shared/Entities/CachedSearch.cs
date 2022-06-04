namespace Shared.Entities;

public class CachedSearch
{
    public string Code { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime UpdateTime { get; set; }
}