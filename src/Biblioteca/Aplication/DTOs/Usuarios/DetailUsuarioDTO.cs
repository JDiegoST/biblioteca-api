using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Aplication.DTOs.Usuarios;
public class DetailUsuarioDTO
{
    public Guid UsuarioId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Rol { get; set; } = null!;
    public string? Direccion { get; set; } = null!;
}
