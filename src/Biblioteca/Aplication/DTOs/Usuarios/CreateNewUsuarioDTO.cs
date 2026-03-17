using Biblioteca.Aplication.DTOs.Direcciones;
using Biblioteca.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Aplication.DTOs.Usuarios;
public class CreateNewUsuarioDTO
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public Rol? Rol { get; set; }

    public CreateDireccionDTO? Direccion { get; set; } = null!;
}
