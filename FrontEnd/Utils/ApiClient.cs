using System.Net;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace FrontEnd.Utils;

public class ApiClient : HttpClient
{
    private ISyncLocalStorageService? _localStorage;
    private NavigationManager? _navigationManager;

    public bool IsLogin
    {
        get;
        private set;
    }

    public void SetAuthorization(string token)
    {
        IsLogin = true;
        var authName =
            "Authorization";
        var hasAuth = this
            .DefaultRequestHeaders
            .Contains(authName);
        if (hasAuth)
        {
            this
                .DefaultRequestHeaders
                .Remove(authName);
        }
        this
            .DefaultRequestHeaders
            .Add(authName, "Bearer " + token);
        
        _localStorage!.SetItemAsString("Auth-Token", token);
    }

    public void SetLocalStorage(ISyncLocalStorageService? syncLocalStorageService)
    {
        _localStorage = syncLocalStorageService;
    }

    public async Task<bool> RefreshToken()
    {
        var response = await GetAsync("api/refresh");
        if (!response.IsSuccessStatusCode)
        {
            IsLogin = false;
            _navigationManager?.NavigateTo("/login");
            return false;
        }

        var readFromJsonAsync = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
        if (readFromJsonAsync == null)
        {
            return false;
        }
        //TODO go to login if can't refresh
        SetAuthorization(readFromJsonAsync.JWTToken);
        return true;
    }

    private async Task<HttpResponseMessage> SendWithRefreshCheck(HttpMethod method, string url, bool firstRun = true)
    {
        var request = new HttpRequestMessage(method, url);
        var response = await SendAsync(request);
        if (!response.IsSuccessStatusCode && firstRun)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshToken = await RefreshToken();
                if (!refreshToken)
                {
                    return response;
                }
                response.Dispose();
                return await SendWithRefreshCheck(method, url, false);
            }
        }
        return response;
    }
    
    private Task<HttpResponseMessage> GetWithRefreshCheck(string url)
    {
        return SendWithRefreshCheck(HttpMethod.Get, url);
    }

/*
    private Task<HttpResponseMessage> PutWithRefreshCheck(string url)
    {
        return SendWithRefreshCheck(HttpMethod.Put, url);
    }
*/

    private Task<HttpResponseMessage> DeleteWithRefreshCheck(string url)
    {
        return SendWithRefreshCheck(HttpMethod.Delete, url);
    }
    
    private Task<HttpResponseMessage> PostWithRefreshCheck(string url)
    {
        return SendWithRefreshCheck(HttpMethod.Post, url);
    }

    public async Task<TickerDetailsDTO?> GetDetails(string? name)
    {
        using var response = await GetWithRefreshCheck($"/api/Stock/{name}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var details = await response.Content.ReadFromJsonAsync<TickerDetailsDTO>();
        return details;
    }

    public async Task<IEnumerable<SearchData>?> GetSearch(string name)
    {
        using var response = await GetWithRefreshCheck($"/api/Stock/search/{name}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var details = await response.Content
            .ReadFromJsonAsync<TickerSearchDTO>();
        
        var searchAutocomplete = details?.Results
            .Select(e => 
                new SearchData(e.Ticker, e.Name, e.PrimaryExchange))
            .ToList();
        return searchAutocomplete;
    }

    public async Task<bool> AddToWatchlist(string name)
    {
        using var response = await PostWithRefreshCheck($"/api/Stock/watchlist/{name}");
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return true;
    }

    public async Task<List<WatchlistData>?> GetWatchlist()
    {
        using var response = await GetWithRefreshCheck($"/api/Stock/watchlist");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var details = await response.Content
            .ReadFromJsonAsync<List<TickerDetailsDTO>>();
        if (details == null)
            return null;
            
        return details.Select(
            e =>
            {
                string imageUrl = "";
                if (e.Results?.Branding != null)
                {
                    imageUrl = e.Results.Branding.LogoUrl?.ToString() ?? "";
                }

                return new WatchlistData
                {
                    ImageUrl = imageUrl,
                    Symbol = e.Results?.Ticker ?? "",
                    Name = e.Results?.Name ?? "",
                    Marker = e.Results?.Market ?? "",
                    Type = e.Results?.Type ?? "",
                };
            }).ToList();
    }

    public async Task<bool> RemoveFromWatchlist(string name)
    {
        using var response = await DeleteWithRefreshCheck($"/api/Stock/watchlist/{name}");
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return true;
    }

    public async Task<List<GraphData>?> GetGraph(string? name, PossibleGraphs type)
    {
        string timespan;
        string from;
        string to;
        var pointInTime = DateTime.Now.AddMonths(-1);
        switch (type)
        {
            default:
            case PossibleGraphs.Today:
                timespan = "day";
                to = pointInTime.ToString("yyyy-MM-dd");
                from = pointInTime.AddDays(-1).ToString("yyyy-MM-dd");
                break;
            case PossibleGraphs.Week:
                timespan = "day";
                to = pointInTime.ToString("yyyy-MM-dd");
                from = pointInTime.AddDays(-7).ToString("yyyy-MM-dd");
                break;
            case PossibleGraphs.Month:
                timespan = "day";
                to = pointInTime.ToString("yyyy-MM-dd");
                from = pointInTime.AddMonths(-1).ToString("yyyy-MM-dd");
                break;
            case PossibleGraphs.Quarter:
                timespan = "week";
                to = pointInTime.ToString("yyyy-MM-dd");
                from = pointInTime.AddMonths(-3).ToString("yyyy-MM-dd");
                break;
        }
        
        using var response = await GetWithRefreshCheck($"/api/Stock/{name}/graph/{timespan}/{from}/{to}");
        if (!response.IsSuccessStatusCode)
        {
            //TODO Handle error
            return null;
        }

        var aggregated = await response.Content.ReadFromJsonAsync<AggregatesDTO>();
        if (aggregated?.Results == null)
        {
            return null;
        }
        
        var result = aggregated.Results.Select(
            e => new GraphData
            {
                X = DateTime.UnixEpoch.AddMilliseconds(e.T),
                Close = e.C,
                High = e.H,
                Low = e.L,
                Open = e.O,
                Volume = e.V
            }).ToList();
        return result;
    }

    public void SetNavigationManager(NavigationManager? navigationManager)
    {
        this._navigationManager = navigationManager;
    }
}

public class GraphData
{        
    public DateTime X { get; set; }
    public double Open { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
    public double High { get; set; }
    public double Volume { get; set; }
}