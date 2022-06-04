using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Server.Utils;

public static class PasswordHashing
{
    public static string HashPassword(string? password, string salt)
    {
        var byteSalt = Convert.FromHexString(salt);
        var passwordHash = KeyDerivation.Pbkdf2(
            password: password ?? throw new ArgumentNullException(nameof(password)),
            salt: byteSalt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 100000,
            numBytesRequested: 512 / 8);
        return Convert.ToHexString(passwordHash);
    }

    /*
     * Generate random salt represented by 64 character string
     */
    public static string GetSalt()
    {
        var randomArray = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(randomArray);
    }

    public static bool Verify(string targetHash, string? password, string salt)
    {
        var hash = HashPassword(password, salt);
        return hash == targetHash;
    }
}