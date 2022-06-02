using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Server.Entities;
using Server.Services;
using Server.Utils;
using Shared.DTO;

namespace Server.Controllers;

[Controller]
[Route("/api")]
public class AccountController : ControllerBase
{
    private readonly StockContext _stockContext;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly CookieOptions opts = new CookieOptions
    {
        Expires = DateTimeOffset.Now.AddDays(7)
    };
    public AccountController(StockContext stockContext, IRefreshTokenService refreshTokenService)
    {
        _stockContext = stockContext;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDataDTO loginData)
    {
        var user = await _stockContext.Accounts
            .Where(e => e.Login == loginData.Login)
            .SingleOrDefaultAsync();
        if (user == null)
        {
            return StatusCode(500, "Can't login");
        }

        if (!PasswordHashing.Verify(user.PasswordHash, loginData.Password, user.PasswordSalt))
        {
            return StatusCode(500, "Can't login");
        }

        var refreshToken = await _refreshTokenService.GetNewTokenAsync(user) ?? "";

        Response.Cookies.Append("Refresh-Token", refreshToken, opts);
        
        return Ok(new LoginResponseDTO
        {
            JWTToken = JWTGenerator.Generate(user)
        });
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDataDTO registerData)
    {
        var user = await _stockContext.Accounts
            .Where(e => e.Login == registerData.Login)
            .SingleOrDefaultAsync();
        if (user != null)
        {
            return Conflict();
        }

        var salt = PasswordHashing.GetSalt();
        var hash = PasswordHashing.HashPassword(registerData.Password, salt);

        try
        {
            await _stockContext.Accounts.AddAsync(new Account
            {
                Login = registerData.Login,
                PasswordHash = hash,
                PasswordSalt = salt
            });
            await _stockContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }

        return Ok();
    }

    [HttpGet]
    [Route("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var headersAuthorization = Request.Headers.Authorization[0];
        if (headersAuthorization == null)
        {
            return BadRequest();
        }

        var refreshToken = Request.Cookies["Refresh-Token"];
        if (refreshToken == null)
        {
            return BadRequest();
        }
        
        // Console.WriteLine(headersAuthorization);
        var token = headersAuthorization.Split(" ")[1];
        var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        int accountId = Request.GetAccountId() ?? -1; 
        if (accountId == -1)
        {
            return BadRequest();
        }
        
        var result = await _refreshTokenService.VerifyTokenAsync(
            refreshToken,
            accountId
        );
        if (!result)
        {
            return Problem();
        }

        var newToken = JWTGenerator.GenerateForPayload(jwtSecurityToken.Payload);
        var newRefreshToken = await _refreshTokenService.GetNewTokenAsync(accountId);
        
        Response.Cookies.Append("Refresh-Token", newRefreshToken, opts);
        return Ok(new LoginResponseDTO
        {
            JWTToken = newToken
        });
    }
}