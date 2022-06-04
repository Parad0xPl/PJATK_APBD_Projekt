using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Entities;

namespace Server.Entities;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder
            .HasKey(e => e.ID);
        builder
            .Property(e => e.ID)
            .UseIdentityColumn();

        builder
            .Property(e => e.Login)
            .HasMaxLength(64);
        builder
            .Property(e => e.PasswordHash)
            .HasMaxLength(128); //TODO Adjust to fit no more than hash
        builder
            .Property(e => e.PasswordSalt)
            .HasMaxLength(128);

        builder
            .HasIndex(e => e.Login)
            .IsUnique();

        builder
            .HasMany(e => e.Watchlist)
            .WithOne();
    }
}