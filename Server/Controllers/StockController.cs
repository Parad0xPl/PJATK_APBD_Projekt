using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Server.Entities;
using Server.Services;
using Shared.DTO;

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

    [HttpPut]
    [Route("watchlist/{name}")]
    public async Task<IActionResult> AddToWatchlist(string name)
    {
        return Ok();
    }
}