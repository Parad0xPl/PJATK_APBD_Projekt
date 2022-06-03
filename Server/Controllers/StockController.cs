using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Server.Entities;
using Server.Services;
using Shared.DTO;
using Server.Utils;

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
                details = JsonSerializer.Deserialize<TickerDetailsDTO>(stocksCache.RequestJSON);
                return Ok(details);
            }
        }

        details = await _stockInfoService.GetDetails(name);
        if (details == null)
        {
            return NotFound();
        }

        var detailsSerialized = JsonSerializer.Serialize(details);

        if (details == null || details.Results == null)
        {
            return NotFound();
        }

        if (stocksCache == null)
        {
            await _stockContext.Stocks.AddAsync(new Stock
            {
                Ticker = details.Results.Ticker,
                RequestJSON = detailsSerialized,
                UpdateTime = DateTime.Now
            });
        }
        else
        {
            stocksCache.RequestJSON = detailsSerialized;
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
        var details = await _stockInfoService.Search(name);
        if (details == null)
        {
            return NotFound();
        }
        return Ok(details);
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
        catch (Exception e)
        {}
            
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
            return JsonSerializer.Deserialize<TickerDetailsDTO>(e.RequestJSON);
        }).ToList();
        return Ok(listDto);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("image/{*.}")]
    public async Task<IActionResult> ProxyImage(string url)
    {
        var image = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAJYAAACWBAMAAADOL2zRAAAAG1BMVEXMzMyWlpaqqqq3t7fFxcW+vr6xsbGjo6OcnJyLKnDGAAAACXBIWXMAAA7EAAAOxAGVKw4bAAABAElEQVRoge3SMW+DMBiE4YsxJqMJtHOTITPeOsLQnaodGImEUMZEkZhRUqn92f0MaTubtfeMh/QGHANEREREREREREREtIJJ0xbH299kp8l8FaGtLdTQ19HjofxZlJ0m1+eBKZcikd9PWtXC5DoDotRO04B9YOvFIXmXLy2jEbiqE6Df7DTleA5socLqvEFVxtJyrpZFWz/pHM2CVte0lS8g2eDe6prOyqPglhzROL+Xye4tmT4WvRcQ2/m81p+/rdguOi8Hc5L/8Qk4vhZzy08DduGt9eVQyP2qoTM1zi0/uf4hvBWf5c77e69Gf798y08L7j0RERERERERERH9P99ZpSVRivB/rgAAAABJRU5ErkJggg==");
        // await Response.Body.WriteAsync(image);
        return new FileContentResult(image, "image/png");
    }
}