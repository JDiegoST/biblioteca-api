using Biblioteca.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public class PrestamoConfiguration
        : BaseEntityConfiguration<Prestamo>
    {
        public override void Configure(EntityTypeBuilder<Prestamo> builder)
        {
            base.Configure(builder);

            builder.ToTable("Prestamos");
            builder.HasKey(p => p.PrestamoId);
            builder.HasOne(p => p.Usuario).WithMany(u => u.Prestamos).HasForeignKey(p => p.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(p => p.FechaPrestamo).IsRequired();
            builder.Property(p => p.FechaVencimiento).IsRequired();
            builder.Property(p => p.FechaDevolucion).IsRequired(false);
        }
    }
}
