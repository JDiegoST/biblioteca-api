using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Aplication.DTOs.Auth
{
    public class LoginResponseDTO
    {
        [Required]
        public string AccessToken { get; set; } = null!;
    }
}
