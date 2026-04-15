using Biblioteca.Aplication.DTOs.Autores;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Libros;

namespace Biblioteca.Aplication.DTOs.Buscador
{
    public class BuscadorResponseDTO
    {
        public PagedResult<DetalleLibroDTO> Libros { get; set; } = null!;
        public PagedResult<DetailAutorDTO> Autores { get; set; } = null!;
    }
}
