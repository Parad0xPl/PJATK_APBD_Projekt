using Shared.DTO;

namespace Server.Services;

public interface IStockInfoService
{
    public Task<TickerDetailsDTO?> GetDetails(string name);
    public Task<AggregatesDTO?> GetGraphData(string name, string timespan, DateOnly from, DateOnly to);
    public Task<TickerSearchDTO?> Search(string phrase);
}