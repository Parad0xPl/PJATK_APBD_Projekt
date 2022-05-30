using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Controller]
[Authorize]
[Route("/api/[controller]")]
public class StockController : ControllerBase
{
    [HttpGet]
    [Route("{name}")]
    public async Task GetStock(string name)
    {
        return;
    }

    [HttpPut]
    [Route("watchlist/{name}")]
    public async Task AddToWatchlist(string name)
    {
        
    }
}