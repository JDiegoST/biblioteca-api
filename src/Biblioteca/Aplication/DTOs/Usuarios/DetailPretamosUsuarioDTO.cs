using Biblioteca.Aplication.DTOs.Prestamos;

namespace Biblioteca.Aplication.DTOs.Usuarios
{
    public class DetailPretamosUsuarioDTO
    {
        public Guid UsuarioId { get; set; }
        public string NombreCompleto {  get; set; } = string.Empty;
        public List<DetailPrestamoDTO> ListPrestamos { get; set; } = null!;
    }
}
