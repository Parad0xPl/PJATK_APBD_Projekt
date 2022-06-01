using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FrontEnd;
using FrontEnd.Utils;
using Blazored.LocalStorage;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var defaultHttpClient = new ApiClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)};
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp =>
{
    var syncLocalStorageService = sp.GetService<ISyncLocalStorageService>();
    defaultHttpClient.SetLocalStorage(syncLocalStorageService);

    var auth = syncLocalStorageService.GetItemAsString("Auth-Token");
    if (auth != null)
    {
        defaultHttpClient.SetAuthorization(auth);
    }

    return defaultHttpClient;
});
builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
await builder.Build().RunAsync();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjQ4ODA1QDMyMzAyZTMxMmUzMFBuVDY0YVZrUXpVNTdMekdYSDNLMzdDbGdqU1A2VU1FQjg0cXZvTm0waFU9");