using Biblioteca.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public class LibroConfiguration 
        : BaseEntityConfiguration<Libro>
    {
        public override void Configure(EntityTypeBuilder<Libro> builder)
        {
            base.Configure(builder);

            builder.ToTable("Libros");
            builder.HasKey(p => p.LibroId);
            builder.Property(p => p.Titulo).IsRequired().HasMaxLength(150);
            builder.Property(p => p.RutaImagen).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Isbn).IsRequired().HasMaxLength(13);
            builder.Property(p => p.AnioPublicacion).IsRequired(false);
            builder.Property(p => p.Editorial).IsRequired().HasMaxLength(50);
            builder.Property(p => p.DescripcionFisica).HasMaxLength(200);
        }
    }
}
