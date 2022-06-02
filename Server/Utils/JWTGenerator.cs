using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Server.Entities;

namespace Server.Utils;

public static class JWTGenerator
{
    private static SigningCredentials? Credentials { get; set; }
    
    public static void Init(string token)
    {        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token));
        Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
    }

    public static string GenerateForPayload(JwtPayload payload)
    {
        var jwtHeader = new JwtHeader(Credentials);
        Claim claimExpDate = new Claim(JwtRegisteredClaimNames.Exp, 
            new DateTimeOffset(DateTime.Now.AddMinutes(5)).ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64);
        payload.Remove(JwtRegisteredClaimNames.Exp);
        payload.AddClaim(claimExpDate);
        var jwtToken = new JwtSecurityToken(jwtHeader, payload);
        
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);;
    }
    
    public static string Generate(Account account)
    {
        var claims = new List<Claim>
        {
            new("ID", account.ID.ToString()),
            new(ClaimTypes.Name, account.Login),
            new(ClaimTypes.Role, "User")
        };


        
        var jwtPayload = new JwtPayload();
        jwtPayload.AddClaims(claims);
        

        return GenerateForPayload(jwtPayload);
    }
}