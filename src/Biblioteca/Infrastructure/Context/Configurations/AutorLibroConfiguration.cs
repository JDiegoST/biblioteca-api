using Biblioteca.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public class AutorLibroConfiguration
    : IEntityTypeConfiguration<AutorLibro>
    {
        public void Configure(EntityTypeBuilder<AutorLibro> builder)
        {
            builder.ToTable("AutorLibro");

            builder.HasKey(x => new { x.AutorId, x.LibroId });

            builder.HasOne(x => x.Autor)
                .WithMany(a => a.AutoresLibros)
                .HasForeignKey(x => x.AutorId);

            builder.HasOne(x => x.Libro)
                .WithMany(l => l.AutoresLibros)
                .HasForeignKey(x => x.LibroId);
        }
    }
}
