using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Server.Entities;

public class CachedImagesConfiguration : IEntityTypeConfiguration<CachedImage>
{
    public void Configure(EntityTypeBuilder<CachedImage> builder)
    {
        builder.HasKey(e => e.Url);
        builder.Property(e => e.Url)
            .HasMaxLength(320);

        builder.Property(e => e.Data)
            .HasMaxLength(4 * 1024 * 1024);

    }
}