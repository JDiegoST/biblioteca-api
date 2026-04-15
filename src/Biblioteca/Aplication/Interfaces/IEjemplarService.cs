using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Ejemplares;

namespace Biblioteca.Aplication.Interfaces
{
    public interface IEjemplarService
    {
        Task<PagedResult<DetailEjemplarDTO>> GetAllEjemplaresAsync(PaginationParams pagination);
        Task<DetailEjemplarDTO?> GetEjemplarByIdAsync(Guid id);
        Task<DetailEjemplarDTO> CreateEjemplarAsync(CreateNewEjemplarDTO ejemplar);
        Task UpdateEjemplarAsync(Guid id, UpdateEjemplarDTO ejemplar);
        Task DeleteEjemplarAsync(Guid id);
    }
}
