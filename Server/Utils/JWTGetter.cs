using System.IdentityModel.Tokens.Jwt;

namespace Server.Utils;

public static class JWTGetter
{
    public static int? GetAccountId(this HttpRequest request)
    {
        var headersAuthorization = request.Headers.Authorization[0];
        if (headersAuthorization == null)
        {
            return null;
        }

        try
        {
            var token = headersAuthorization.Split(" ")[1];
            var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var accountId = Convert.ToInt32(
                jwtSecurityToken.Payload["ID"]
            );
            return Convert.ToInt32(accountId);
        }
        catch (Exception)
        {
            return null;
        }
    }
}