namespace Biblioteca.Aplication.DTOs.IdentityUsuario
{
    public class IdentityUsuarioDTO
    {
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string UserRol { get; set; } = null!;
    }
}
