using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Server.Entities;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder
            .HasKey(e => e.Id);
        builder
            .Property(e => e.Id)
            .UseIdentityColumn();

        builder
            .Property(e => e.Token)
            .HasMaxLength(128);

        builder
            .HasOne<Account>(e => e.Account)
            .WithOne()
            .HasForeignKey<RefreshToken>(e => e.AccountID);
    }
}