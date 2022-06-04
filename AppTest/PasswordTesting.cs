using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NUnit.Framework;
using Server.Utils;

namespace AppTest;

public class PasswordTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public Task RandomPassword()
    {
        var password = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
        
        var salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        var hashA = PasswordHashing.HashPassword(password, salt);
        var hashB = PasswordHashing.HashPassword(password, salt);

        Assert.AreEqual(hashA, hashB);
        
        Assert.True(PasswordHashing.Verify(hashA, password, salt));
        return Task.CompletedTask;
    }
}