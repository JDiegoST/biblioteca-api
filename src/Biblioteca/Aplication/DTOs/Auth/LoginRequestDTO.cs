using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Biblioteca.Aplication.DTOs.Auth
{
    public class LoginRequestDTO
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
