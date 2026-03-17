using Biblioteca.Domain.Models.Auth;
using Biblioteca.Domain.Models.Common;
using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Domain.Models;
public class Usuario : BaseModel 
{
    public Usuario() {}
    public Guid UsuarioId { get; set; }
    public Guid IdentityUserId { get; set; } = default!;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto { get => $"{Nombre} {Apellido}"; }

    public AplicationUser IdentityUser { get; set; } = default!;
    public Direccion Direccion { get; set; } = null!;
    public ICollection<Prestamo> Prestamos { get; set; } = [];

    public override string ToString()
    {
        return "";
    }
    
}
