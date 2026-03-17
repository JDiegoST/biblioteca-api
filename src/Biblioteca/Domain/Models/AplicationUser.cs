using Biblioteca.Domain.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Domain.Models
{
    public class AplicationUser : IdentityUser<Guid> 
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
