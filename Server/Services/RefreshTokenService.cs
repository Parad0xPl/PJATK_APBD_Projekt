using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Server.Entities;
using Shared.Entities;

namespace Server.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly StockContext _stockContext;

    public RefreshTokenService(StockContext stockContext)
    {
        _stockContext = stockContext;
    }

    public Task<string?> GetNewTokenAsync(Account account)
    {
        return GetNewTokenAsync(account.ID);
    }

    public async Task<string?> GetNewTokenAsync(int accountId)
    {        
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var existingToken = await _stockContext.RefreshTokens
            .Where(e => e.AccountID == accountId)
            .SingleOrDefaultAsync();

        if (existingToken != null)
        {
            existingToken.Token = token;
            await _stockContext.SaveChangesAsync();
            return token;
        }
        
        try
        {
            await _stockContext.RefreshTokens.AddAsync(new RefreshToken
            {
                AccountID = accountId,
                Token = token
            });
            await _stockContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

        return token;
    }

    public async Task<bool> VerifyTokenAsync(string token, int accountId)
    {
        var t = await _stockContext
            .RefreshTokens
            .Where(e => e.Token == token)
            .SingleOrDefaultAsync();
        var result = t != null && t.AccountID == accountId;
        if (t != null && t.AccountID == accountId)
        {
            _stockContext.RefreshTokens.Remove(t);
            await _stockContext.SaveChangesAsync();
        }
        return result;
    }
}