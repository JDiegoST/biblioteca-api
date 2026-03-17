using Biblioteca.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public class AutoConfiguration 
        : BaseEntityConfiguration<Autor>
    {
        public override void Configure(EntityTypeBuilder<Autor> builder)
        {
            base.Configure(builder);

            builder.ToTable("Autores");
            builder.HasKey(p => p.AutorId);
            builder.Property(p => p.Nombre).IsRequired().HasMaxLength(30);
            builder.Property(p => p.Apellido).IsRequired().HasMaxLength(30);
            builder.Property(p => p.Biografia).HasMaxLength(500);
            builder.Property(p => p.UrlImgAutor).IsRequired(false).HasMaxLength(200);
        }
    }
}
