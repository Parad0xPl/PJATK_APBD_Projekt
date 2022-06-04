using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Server.Entities;

public class StockContext : DbContext
{
    public virtual DbSet<Account> Accounts {get;set;} = null!;
    public virtual DbSet<ObservedStock> Watchlist {get;set;} = null!;
    public virtual DbSet<Stock> Stocks {get;set;} = null!;
    public virtual DbSet<RefreshToken> RefreshTokens {get;set;} = null!;
    public virtual DbSet<CachedImage> Cached { get; set; } = null!;
    public virtual DbSet<CachedSearch> CachedSearches { get; set; } = null!;

    public StockContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder
            .ApplyConfigurationsFromAssembly(
                typeof(AccountConfiguration).GetTypeInfo().Assembly
            );
    }
}