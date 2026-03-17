using Biblioteca.Aplication.DTOs.Direcciones;
using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Aplication.DTOs.Usuarios;
public class UpdateUsuarioDTO
{
    public string? Nombre { get; set; } = string.Empty;
    public string? Apellido { get; set; } = string.Empty;
    public Rol? Rol { get; set; }
    public UpdateDomicilioDTO? NewDireccion {  get; set; }
}

