using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Entities;

namespace Server.Entities;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Ticker)
            .HasMaxLength(10); // Better more space then less
        builder
            .Property(e => e.RequestJson)
            .HasMaxLength(4000); // Fit whole json

        builder.HasIndex(e => e.Ticker)
            .IsUnique();

        builder.HasMany<ObservedStock>(e => e.Observers)
            .WithOne();
    }
}