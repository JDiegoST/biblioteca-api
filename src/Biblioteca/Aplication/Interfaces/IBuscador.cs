using Biblioteca.Aplication.DTOs.Buscador;
using Biblioteca.Aplication.DTOs.common;

namespace Biblioteca.Aplication.Interfaces
{
    public interface IBuscador
    {
        Task<BuscadorResponseDTO> GetSearchingAsync(FilterParams filters, PaginationParams pagination);
    }
}
