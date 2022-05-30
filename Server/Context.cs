using Microsoft.EntityFrameworkCore;

namespace Server.Entities;

public class Context : DbContext
{
    public DbSet<Account> Accounts;
    public DbSet<ObservedStock> Watchlist;
    public DbSet<Stock> Stocks;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}