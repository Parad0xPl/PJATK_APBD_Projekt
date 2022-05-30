namespace Server.Services;

public interface IStockInfoService
{
    public Task GetDetails(string name);
    public Task GetGraphData(string name);
    public Task GetNames();
}