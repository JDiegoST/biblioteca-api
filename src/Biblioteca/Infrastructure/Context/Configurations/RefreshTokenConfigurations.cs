using Biblioteca.Domain.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public class RefreshTokenConfigurations
        : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> entity)
        {
            entity.ToTable("RefreshTokens");

            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.TokenHash)
                  .IsRequired()
                  .HasMaxLength(512);

            entity.Property(rt => rt.IdentityUserId)
                  .IsRequired();

            entity.Property(rt => rt.CreatedByIp)
                  .HasMaxLength(50);

            entity.Property(rt => rt.CreatedAt)
                  .IsRequired();

            entity.Property(rt => rt.ExpiresAt)
                  .IsRequired();

            entity.HasOne(rt => rt.IdentityUser)
                  .WithMany(id => id.RefreshTokens)
                  .HasForeignKey(rt => rt.IdentityUserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<RefreshToken>()
                  .WithMany()
                  .HasForeignKey(rt => rt.ReplacedByTokenId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Ignore(rt => rt.IsExpired);

            entity.HasIndex(rt => rt.TokenHash).IsUnique();
            entity.HasIndex(rt => rt.IdentityUserId);
        }
    }
}
