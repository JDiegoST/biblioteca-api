using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Prestamos;

namespace Biblioteca.Aplication.Interfaces
{
    public interface IPrestamoService
    {
        Task<PagedResult<DetailPrestamoDTO>> GetAllLoansAsync(PaginationParams pagination);
        Task<DetailPrestamoDTO?> GetLoanByIdAsync(Guid id);
        Task<DetailPrestamoDTO> CreateNewLoanAsync(CreateNewPrestamoDTO prestamo);
        Task UpdateLoanAsync(Guid id, UpdatePrestamoDTO prestamo);

        Task GoBackLoanAsync(Guid id);
    }
}
