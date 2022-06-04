using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Server.Entities;

public class CachedSearchesConfiguration : IEntityTypeConfiguration<CachedSearch>
{
    public void Configure(EntityTypeBuilder<CachedSearch> builder)
    {
        builder
            .HasKey(e => e.Code);

        builder
            .Property(e => e.Code)
            .HasMaxLength(2);
        builder
            .Property(e => e.Message)
            .HasMaxLength(1024 * 1024);
    }
}