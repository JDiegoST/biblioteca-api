using Biblioteca.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public class DireccionConfiguration
        : IEntityTypeConfiguration<Direccion>
    {
        public void Configure(EntityTypeBuilder<Direccion> builder)
        {
            builder.ToTable("Direcciones");
            builder.HasKey(d => d.UsuarioId);

            builder.Property(d => d.Pais).HasMaxLength(3).HasDefaultValue("MEX");
            builder.Property(d => d.Estado).IsRequired().HasMaxLength(30);
            builder.Property(d => d.CodigoPostal).IsRequired().HasMaxLength(5);
            builder.Property(d => d.Municipio).IsRequired().HasMaxLength(30);
            builder.Property(d => d.Colonia).IsRequired().HasMaxLength(20);
            builder.Property(d => d.Calle).IsRequired().HasMaxLength(30);
            builder.Property(d => d.NumeroExterior).IsRequired();
            builder.Property(d => d.NumeroInterior).IsRequired(false);
        }
    }
}
