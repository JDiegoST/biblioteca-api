using Biblioteca.Domain.Models;
using Biblioteca.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public class EjemplarConfiguration
        : BaseEntityConfiguration<Ejemplar>
    {
        public override void Configure(EntityTypeBuilder<Ejemplar> builder)
        {
            base.Configure(builder);

            builder.ToTable("Ejemplares");
            builder.HasKey(e => e.EjemplarId);
            builder.HasOne(e => e.Libro).WithMany(l => l.Ejemplares).HasForeignKey(e => e.LibroId)
             .OnDelete(DeleteBehavior.Restrict);
            builder.Property(e => e.CodigoInventario).IsRequired().HasMaxLength(13);
            builder.HasMany(e => e.Prestamos).WithOne(p => p.Ejemplar).HasForeignKey(p => p.EjemplarId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.Estado).HasConversion<int>().IsRequired().HasDefaultValue(EstadoPrestamo.Disponible);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Ejemplar_Estado", "Estado in (1, 2)"));

            builder.HasIndex(e => e.CodigoInventario).IsUnique();
        }
    }
}
