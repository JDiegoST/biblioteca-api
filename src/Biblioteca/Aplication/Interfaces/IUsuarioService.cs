using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Direcciones;
using Biblioteca.Aplication.DTOs.Prestamos;
using Biblioteca.Aplication.DTOs.Usuarios;

namespace Biblioteca.Aplication.Interfaces
{
    public interface IUsuarioService
    {
        Task<PagedResult<DetailUsuarioDTO>> GetUsuariosAsync(PaginationParams pagination);
        Task<DetailUsuarioDTO?> GetUsuarioByIdAsync(Guid id);
        Task<PagedResult<DetailPrestamoDTO>> GetPrestamosUsuarioByIdAsync(Guid id, PaginationParams pagination);
        Task<DetailUsuarioDTO> CreateUsuarioAsync(CreateNewUsuarioDTO usuario);
        Task<DetailDomicilioDTO> AsingDomicilioToUser(Guid userId, CreateDireccionDTO direccion);
        Task UpdateUsuarioAsync(Guid id, UpdateUsuarioDTO usuario);
        Task DeleteUsuarioAsync(Guid id);
    }
}
