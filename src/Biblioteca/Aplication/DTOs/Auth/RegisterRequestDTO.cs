using Biblioteca.Aplication.DTOs.Usuarios;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Aplication.DTOs.Auth
{
    public class RegisterRequestDTO
    {

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = null!;
        [Required]
        public string Password {  get; set; } = null!;

        public string? Nombre { get; set; } = null!;
        public string? Apellido { get; set; } = null!;

    }
}
