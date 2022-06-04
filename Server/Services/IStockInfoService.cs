using Shared.DTO;

namespace Server.Services;

public interface IStockInfoService
{
    public Task<TickerDetailsDTO?> GetDetails(string name, Uri hostUri);
    public Task<AggregatesDTO?> GetGraphData(string name, string timespan, DateOnly from, DateOnly to);
    public Task<TickerSearchDTO?> Search(string phrase);
    public Task<Tuple<string?, byte[]>?> GetImage(string url);
}