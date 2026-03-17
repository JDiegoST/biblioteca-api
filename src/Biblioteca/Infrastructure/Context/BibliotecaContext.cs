using Biblioteca.Domain.Interfaces;
using Biblioteca.Domain.Models;
using Biblioteca.Domain.Models.Auth;
using Biblioteca.Domain.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Biblioteca.Infrastructure.Context;
public class BibliotecaContext(DbContextOptions<BibliotecaContext> options)
    : IdentityDbContext<AplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Autor> Autores { get; set; }
    public DbSet<AutorLibro> AutorLibros { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Direccion> Direcciones { get; set; }
    public DbSet<Ejemplar> Ejemplares { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Me permite aplicar todas las configuraciones hechas en la carpeta
        // .\configurations\ para cada entidad que hereden de IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BibliotecaContext).Assembly);

        // Filtro global para todas las query que EF genere.
        // Evita que cada vez que haga consultas SELECT tenga que incluir DeletedAt == null
        // para conseguir los registros no borrados logicamente por SOFT DELETE
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeleted).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeleted.DeletedAt));
                var condition = Expression.Equal(
                    property,
                    Expression.Constant(null)
                );

                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(lambda);
            }
        }

    }


    /// <summary>
    /// Se Sobreescribe el metodo con el objetivo de centralizar el llenado de
    /// los campos de auditoria de todas las entidades del Contexto 
    /// ante cualquier insercion, actualizacion o borrado que se realice.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseModel>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                // Asi, si se agrega un nuevo registro, toma el campo CreatedAt y lo mapea con la 
                // fecha actual
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                // Pasa exactamente lo mismo ante una actualizacion.
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Convertimos DELETE en SOFT DELETE cuando invocamos el metodo .Remove()
                    entry.State = EntityState.Modified;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    // NOTA: Ante un borrado, como el objetivo es preservar los datos en la BD,
    // debemos de mapear DeletedAt con la DateTime de cuando se realizo el borrado.
    // Normalmente, el motodo .Remove() ejecuta un DELETE de sql para el registro, pero esta
    // sobreescritura lo cambia a una actualizacion del campo DeleteAt ya que SaveChangesAsync()
    // es el Metodo que le dice a EF que ejecute el sql en la base de datos.
    // DEBE SE EVITARSE A TODA COSTA Y TENER MUCHO CUIDADO CUALQUIER METODO QUE EJECUTE
    // SQL DIRECTAMENTE EN EL CONTEXTO DEL EF EN ESTE PROYECTO.
    // Ya que al no verse obligado de pasar por el metodo SaveChangesAsync() para guardar cambios en la BD
    // no cumplira con su funcion descrita causando posibles errores.

}
