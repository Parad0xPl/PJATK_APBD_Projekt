using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Entities;

namespace Server.Entities;

public class ObservedStockConfiguration : IEntityTypeConfiguration<ObservedStock>
{
    public void Configure(EntityTypeBuilder<ObservedStock> builder)
    {
        builder
            .HasKey(e => new { e.AccountId, e.StockId });
    }
}