namespace Biblioteca.Aplication.DTOs.Auth
{
    public class RegisterResponseDTO
    {
        public Guid? UsuarioId {  get; set; }
        public string Email { get; set; } = null!;
        public string? NombreCompleto { get; set; } = null!;
        public string Rol { get; set; } = null!;
}
}
