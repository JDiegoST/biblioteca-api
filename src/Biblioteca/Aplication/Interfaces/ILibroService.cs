using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Libros;

namespace Biblioteca.Aplication.Interfaces
{
    public interface ILibroService
    {
        Task<PagedResult<DetalleLibroDTO>> GetLibrosAsync(FilterParams filters, PaginationParams pagination);
        Task<DetalleLibroDTO?> GetLibroByIdAsync(Guid id);
        Task<DetalleLibroDTO> CreateNewLibroAsync(CreateNewLibroDTO newlibro);
        Task UpdateLibroAsync(Guid id, UpdateLibroDTO libroToUpdate);
        Task DeleteLibroAsync(Guid id);
    }
}
