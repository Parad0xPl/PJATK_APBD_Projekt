using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Controller]
[Route("/api")]
public class LoginController : ControllerBase
{
    [HttpPost]
    [Route("/login")]
    public async Task Login()
    {
        
    }

    [HttpPost]
    [Route("/register")]
    public async Task Register()
    {
        
    }

    [HttpPost]
    [Route("/refresh")]
    public async Task RefreshToken()
    {
        
    }
}