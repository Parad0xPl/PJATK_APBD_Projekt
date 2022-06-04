using System.Text.Json;
using Microsoft.Net.Http.Headers;
using Shared.DTO;

namespace Server.Services;

public class StockInfoService : IStockInfoService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.polygon.io";
    private const string TickerEndpoint = "/v3/reference/tickers";
    private const string SearchEndpoint = "/v3/reference/tickers?limit=1000&search=";
    private const string AggregateEndpoint = "/v2/aggs/ticker/{3}/range/1/{0}/{1}/{2}";
    private const string ImageEndpoint = "/v1/reference";

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
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiUrl);
        _httpClient.DefaultRequestHeaders.Add(
            HeaderNames.Authorization, "Bearer " + apiKey);
    }
    public StockInfoService(HttpClient httpClient, IConfiguration configuration)
    {
        var apiKey = configuration["PolygonAPIKey"];
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiUrl);
        _httpClient.DefaultRequestHeaders.Add(
            HeaderNames.Authorization, "Bearer " + apiKey);
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
        var messageString = await response.Content.ReadAsStreamAsync();
        var ticker = await JsonSerializer.DeserializeAsync<TickerSearchDTO>(messageString);
        return ticker;
    }

    public async Task<Tuple<string?, byte[]>?> GetImage(string url)
    {
        var response = await _httpClient.GetAsync(ImageEndpoint + "/" + url);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        string? contentType = null;

        if (response.Content.Headers.ContentType != null)
        {
            contentType = response.Content.Headers.ContentType.ToString();
        }
        var messageString = await response.Content.ReadAsByteArrayAsync();
        return new Tuple<string?, byte[]>(contentType, messageString);
    }
}