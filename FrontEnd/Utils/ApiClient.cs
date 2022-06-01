using System.Net;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Shared.DTO;

namespace FrontEnd.Utils;

public class ApiClient : HttpClient
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

    public async Task<TickerDetailsDTO?> GetDetails(string name)
    {
        using var response = await this.GetAsync($"/api/Stock/{name}");

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshToken = await RefreshToken();
                if (!refreshToken)
                {
                    return null;
                }

                return await GetDetails(name);
            }
            else
            {
                
                return null;
            }
        }

        var details = await response.Content.ReadFromJsonAsync<TickerDetailsDTO>();
        return details;

    }
}