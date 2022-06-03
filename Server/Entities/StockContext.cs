using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Server.Entities;

public class StockContext : DbContext
{
    public virtual DbSet<Account> Accounts {get;set;}
    public virtual DbSet<ObservedStock> Watchlist {get;set;}
    public virtual DbSet<Stock> Stocks {get;set;}
    public virtual DbSet<RefreshToken> RefreshTokens {get;set;}
    public virtual DbSet<CachedImage> Cached { get; set; }

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