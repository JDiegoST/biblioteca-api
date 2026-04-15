using Biblioteca.Aplication.DTOs.Autores;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Libros;

namespace Biblioteca.Aplication.Interfaces
{
    public interface IAutorService
    {
        Task<PagedResult<DetailAutorDTO>> GetAutorsAsync(FilterParams filters, PaginationParams pagination);
        Task<DetailAutorDTO?> GetAutorByIdAsync(Guid id);

        Task<PagedResult<SummaryLibroResumenDTO>> GetLibrosOfAutor(Guid autorId, PaginationParams pagination);
        Task<DetailAutorDTO> SaveAutorAsync(SaveAutorDTO autor);
        Task UpdateAutorAsync(Guid id, UpdateAutorDTO autor);
        Task DeleteAutorAsync(Guid id);
    }
}
