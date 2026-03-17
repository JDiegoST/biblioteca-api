using Biblioteca.Domain.Models.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public abstract class BaseEntityConfiguration<T>
    : IEntityTypeConfiguration<T>
    where T : BaseModel
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime(6)")
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            builder.Property(e => e.UpdatedAt)
                .HasColumnType("datetime(6)");

            builder.Property(e => e.DeletedAt)
                .HasColumnType("datetime(6)");

            builder.HasIndex(e => e.DeletedAt);
        }
    }
}
