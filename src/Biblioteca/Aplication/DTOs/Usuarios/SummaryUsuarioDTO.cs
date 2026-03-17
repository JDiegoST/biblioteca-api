using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Aplication.DTOs.Usuarios;
public class SummaryUsuarioDTO
{
    public Guid UsuarioId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public Rol? Rol {  get; set; }
}
