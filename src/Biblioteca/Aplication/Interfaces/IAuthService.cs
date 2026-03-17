using Biblioteca.Aplication.DTOs.Auth;

namespace Biblioteca.Aplication.Interfaces
{
    public interface IAuthService
    {
        const short DIAS_A_EXPIRAR = 7;
        Task<TokenResultDTO> LoginAsync(LoginRequestDTO dto, string? ipAddress);

        Task<RegisterResponseDTO> AuthRegisterAsync(RegisterRequestDTO dto, string origin);

        Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress);

        Task RevokeRefreshTokenAsync(string refreshToken, string ipAddress);
    }
}
