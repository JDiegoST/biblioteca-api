using Biblioteca.Domain.Models;
using Biblioteca.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Context.Configurations
{
    public class UsuarioConfiguration
        : BaseEntityConfiguration<Usuario>
    {
        public override void Configure(EntityTypeBuilder<Usuario> builder)
        {
            base.Configure(builder);

            builder.ToTable("Usuarios");
            builder.HasKey(u => u.UsuarioId);
            builder.HasOne(u => u.Direccion)
             .WithOne(d => d.Usuario).HasForeignKey<Direccion>(d => d.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);
            builder.Property(u => u.Nombre).IsRequired().HasMaxLength(30);
            builder.Property(u => u.Apellido).IsRequired().HasMaxLength(30);
            builder.HasOne(u => u.IdentityUser)
                .WithOne()
                .HasForeignKey<Usuario>(u => u.IdentityUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
