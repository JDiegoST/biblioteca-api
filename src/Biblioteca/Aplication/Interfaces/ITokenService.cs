using System.Security.Claims;

namespace Biblioteca.Aplication.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}
