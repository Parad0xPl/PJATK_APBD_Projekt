using Shared.Entities;

namespace Server.Services;

public interface IRefreshTokenService
{
    public Task<string?> GetNewTokenAsync(Account account);
    public Task<string?> GetNewTokenAsync(int accountId);
    public Task<bool> VerifyTokenAsync(string token, int accountId);
}