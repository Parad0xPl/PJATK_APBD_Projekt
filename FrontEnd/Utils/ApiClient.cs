using System.Net;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Shared.DTO;

namespace FrontEnd.Utils;

public partial class ApiClient : HttpClient
{
    private ISyncLocalStorageService? _localStorage;

    public void SetAuthorization(string token)
    {
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

    public void SetLocalStorage(ISyncLocalStorageService syncLocalStorageService)
    {
        _localStorage = syncLocalStorageService;
    }

    public async Task<bool> RefreshToken()
    {
        var response = await GetAsync("api/refresh");
        if (!response.IsSuccessStatusCode)
        {
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

    private Task<HttpResponseMessage> PutWithRefreshCheck(string url)
    {
        return SendWithRefreshCheck(HttpMethod.Put, url);
    }

    private Task<HttpResponseMessage> PostWithRefreshCheck(string url)
    {
        return SendWithRefreshCheck(HttpMethod.Post, url);
    }

    public async Task<TickerDetailsDTO?> GetDetails(string name)
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
        
        var searchAutocomplete = details
            .Results
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
        return details.Select(
            e => new WatchlistData
            {
                ImageURL = e.Results.Branding.LogoUrl.ToString(),
                Symbol = e.Results.Ticker,
                Name = e.Results.Name,
                Marker = e.Results.Market,
                Type = e.Results.Type,
            }
            ).ToList();
    }
}