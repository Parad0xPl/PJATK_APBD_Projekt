using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;
using Shared.DTO;

namespace Server.Services;

public class StockInfoService : IStockInfoService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string ApiUrl = "https://api.polygon.io";
    private const string TickerEndpoint = "/v3/reference/tickers";
    private const string SearchEndpoint = "/v3/reference/tickers?limit=10&search=";
    private const string AggregateEndpoint = "/v2/aggs/ticker/{3}/range/1/{0}/{1}/{2}";

    private string GetAggregateEndpoint(string timespan, DateOnly from, DateOnly to, string name)
    {
        return String.Format(AggregateEndpoint, 
            timespan, 
            from.ToString("yyyy-MM-dd"),
            to.ToString("yyyy-MM-dd"), 
            name);
    }
    public StockInfoService(HttpClient httpClient, string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiUrl);
        _httpClient.DefaultRequestHeaders.Add(
            HeaderNames.Authorization, "Bearer " + _apiKey);
    }
    public StockInfoService(HttpClient httpClient, IConfiguration configuration)
    {
        _apiKey = configuration["PolygonAPIKey"];
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiUrl);
        _httpClient.DefaultRequestHeaders.Add(
            HeaderNames.Authorization, "Bearer " + _apiKey);
    }

    public async Task<TickerDetailsDTO?> GetDetails(string name)
    {
        var response = await _httpClient.GetAsync(TickerEndpoint + "/" + name);
        var messageStream = await response.Content.ReadAsStreamAsync();
        var ticker = await JsonSerializer.DeserializeAsync<TickerDetailsDTO>(messageStream);
        if (ticker != null && ticker.Results != null && ticker.Results.Branding != null)
        {
            if (ticker.Results.Branding.IconUrl != null)
            {
                ticker.Results.Branding.IconUrl = ChangeToProxy(ticker.Results.Branding.IconUrl);
            }

            if (ticker.Results.Branding.LogoUrl != null)
            {
                ticker.Results.Branding.LogoUrl = ChangeToProxy(ticker.Results.Branding.LogoUrl);
            }
        }
        return ticker;
    }

    private Uri ChangeToProxy(Uri brandingIconUrl)
    {
        var builder = new UriBuilder(brandingIconUrl);
        builder.Scheme = "http";
        builder.Host = "localhost";
        builder.Port = 5290;
        builder.Path = "/api/stock/image"+builder.Path.Substring(13);
        return builder.Uri;
    }

    public async Task<AggregatesDTO?> GetGraphData(string name, string timespan, DateOnly from, DateOnly to)
    {        
        var response = await _httpClient.GetAsync(GetAggregateEndpoint(timespan, from, to, name));
        var messageStream = await response.Content.ReadAsStreamAsync();
        var ticker = await JsonSerializer.DeserializeAsync<AggregatesDTO>(messageStream);
        return ticker;
    }

    public async Task<TickerSearchDTO?> Search(string phrase)
    {
        var response = await _httpClient.GetAsync(SearchEndpoint + phrase);
        // var message = await response.Content.ReadAsStringAsync();
        // var ticker = JsonSerializer.Deserialize<TickerSearchDTO>(message);
        var messageString = await response.Content.ReadAsStreamAsync();
        var ticker = await JsonSerializer.DeserializeAsync<TickerSearchDTO>(messageString);
        return ticker;
    }
}