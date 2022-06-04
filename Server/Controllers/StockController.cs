using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Entities;
using Server.Services;
using Shared.DTO;
using Server.Utils;
using Shared.Entities;

namespace Server.Controllers;

[Controller]
[Authorize]
[Route("/api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockInfoService _stockInfoService;
    private readonly StockContext _stockContext;

    public StockController(IStockInfoService stockInfoService, StockContext stockContext)
    {
        _stockInfoService = stockInfoService;
        _stockContext = stockContext;
    }

    [HttpGet]
    [Route("{name}")]
    public async Task<IActionResult> GetStock(string name)
    {
        var stocksCache = 
            await _stockContext.Stocks
                .Where(e => e.Ticker == name)
                .SingleOrDefaultAsync();

        TickerDetailsDTO? details;
        if (stocksCache != null)
        {
            if (stocksCache.UpdateTime.AddHours(1).CompareTo(DateTime.Now) > 0)
            {
                details = JsonSerializer.Deserialize<TickerDetailsDTO>(stocksCache.RequestJson);
                return Ok(details);
            }
        }

        details = await _stockInfoService.GetDetails(name);
        if (details == null)
        {
            return NotFound();
        }

        var detailsSerialized = JsonSerializer.Serialize(details);

        if (details.Results?.Ticker == null)
        {
            return NotFound();
        }

        if (stocksCache == null)
        {
            await _stockContext.Stocks.AddAsync(new Stock
            {
                Ticker = details.Results.Ticker,
                RequestJson = detailsSerialized,
                UpdateTime = DateTime.Now
            });
        }
        else
        {
            stocksCache.RequestJson = detailsSerialized;
            stocksCache.UpdateTime = DateTime.Now;
        }
        await _stockContext.SaveChangesAsync();
        return Ok(details);
    }
    
    [HttpGet]
    [Route("{name}/graph/{timespan}/{from}/{to}")]
    public async Task<IActionResult> GetGraph(string name, string timespan, string from, string to)
    {
        DateOnly fromDate = DateOnly.Parse(from);
        DateOnly toDate = DateOnly.Parse(to);
        var details = await _stockInfoService.GetGraphData(name, timespan, fromDate, toDate);
        if (details == null)
        {
            return NotFound();
        }
        return Ok(details);
    }
    
    [HttpGet]
    [Route("search/{name}")]
    public async Task<IActionResult> SearchStock(string name)
    {
        var code = name.Substring(0, 2);
        if (code.Length < 2)
        {
            return BadRequest();
        }

        var cachedResult = await _stockContext
            .CachedSearches
            .Where(e => e.Code == code)
            .SingleOrDefaultAsync();
        if (cachedResult != null && 
            cachedResult.UpdateTime.AddDays(1).CompareTo(DateTime.Now) >= 0)
        {
            var result = JsonSerializer.Deserialize<TickerSearchDTO>(cachedResult.Message);
            return Ok(FilterSearch(result, name));
        }
        
        var details = await _stockInfoService.Search(code);
        if (details == null)
        {
            return NotFound();
        }

        var message = JsonSerializer.Serialize(details);
        if (cachedResult != null)
        {
            cachedResult.Message = message;
            cachedResult.UpdateTime = DateTime.Now;
        }
        else
        {
            await _stockContext
                .CachedSearches
                .AddAsync(
                    new CachedSearch
                    {
                        Code = code,
                        Message = message,
                        UpdateTime = DateTime.Now
                    }
                );
        }

        await _stockContext.SaveChangesAsync();
        
        return Ok(FilterSearch(details, name));
    }

    private TickerSearchDTO? FilterSearch(TickerSearchDTO? result, string name)
    {
        if (result?.Results == null)
        {
            return null;
        }
        result.Results = result
            .Results
            .Where(e => e.Ticker != null && e.Ticker.StartsWith(name))
            .ToList();
        return result;
    }

    [HttpPost]
    [Route("watchlist/{name}")]
    public async Task<IActionResult> AddToWatchlist(string name)
    {
        
        var stock = await _stockContext.Stocks
            .Where(e => e.Ticker == name)
            .SingleOrDefaultAsync();
        if (stock == null)
        {
            return NotFound();
        }

        var userId = Request.GetAccountId() ?? -1;
        if (userId == -1)
        {
            return Unauthorized();
        }

        try
        {
            await _stockContext
                .Watchlist
                .AddAsync(new ObservedStock
                {
                    AccountId = userId,
                    StockId = stock.Id
                });
            await _stockContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            // ignored
        }

        return Ok();
    }

    [HttpDelete]
    [Route("watchlist/{name}")]
    public async Task<IActionResult> RemoveToWatchlist(string name)
    {
        var userId = Request.GetAccountId() ?? -1;
        if (userId == -1)
        {
            return Unauthorized();
        }

        var stock = await _stockContext
            .Stocks
            .Where(e => e.Ticker == name)
            .SingleOrDefaultAsync();

        if (stock == null)
        {
            return NotFound();
        }

        var watchlistRow = await _stockContext
            .Watchlist
            .Where(e => e.AccountId == userId && e.StockId == stock.Id)
            .SingleOrDefaultAsync();
        if (watchlistRow == null)
        {
            return NotFound();
        }
        _stockContext.Watchlist.Remove(watchlistRow);
        await _stockContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route("watchlist")]
    public async Task<IActionResult> GetWatchlist()
    {
        var userId = Request.GetAccountId() ?? -1;
        if (userId == -1)
        {
            return Unauthorized();
        }

        var list = await _stockContext
            .Watchlist
            .Where(e => e.AccountId == userId)
            .Join(_stockContext.Stocks, e => e.StockId, e => e.Id, (e, s) => s)
            .ToListAsync();

        var listDto = list.Select(e =>
        {
            return JsonSerializer.Deserialize<TickerDetailsDTO>(e.RequestJson);
        }).ToList();
        return Ok(listDto);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("image/{*url}")]
    public async Task<IActionResult> ProxyImage(string url)
    {
        var images = await _stockContext
            .Cached
            .Where(e => e.Url == url)
            .SingleOrDefaultAsync();
        if (images != null)
        {
            return new FileContentResult(images.Data, images.Type);
        }

        var image = await _stockInfoService.GetImage(url);
        // var image = Convert.FromBase64String(
        //     "iVBORw0KGgoAAAANSUhEUgAAAJYAAACWBAMAAADOL2zRAAAAG1BMVEXMzMyWlpaqqqq3t7fFxcW+vr6xsbGjo6OcnJyLKnDGAAAACXBIWXMAAA7EAAAOxAGVKw4bAAABAElEQVRoge3SMW+DMBiE4YsxJqMJtHOTITPeOsLQnaodGImEUMZEkZhRUqn92f0MaTubtfeMh/QGHANEREREREREREREtIJJ0xbH299kp8l8FaGtLdTQ19HjofxZlJ0m1+eBKZcikd9PWtXC5DoDotRO04B9YOvFIXmXLy2jEbiqE6Df7DTleA5socLqvEFVxtJyrpZFWz/pHM2CVte0lS8g2eDe6prOyqPglhzROL+Xye4tmT4WvRcQ2/m81p+/rdguOi8Hc5L/8Qk4vhZzy08DduGt9eVQyP2qoTM1zi0/uf4hvBWf5c77e69Gf798y08L7j0RERERERERERH9P99ZpSVRivB/rgAAAABJRU5ErkJggg==");
        // await Response.Body.WriteAsync(image);
        if (image == null)
        {
            return NotFound();
        }

        var imageData = image.Item2;
        var imageType = image.Item1 ?? "image/png";

        await _stockContext
            .Cached
            .AddAsync(new CachedImage
            {
                Url = url,
                Type = imageType,
                Data = imageData,
                CreationDate = DateTime.Now
            });
        await _stockContext.SaveChangesAsync();
        
        return new FileContentResult(imageData, imageType);
    }
}