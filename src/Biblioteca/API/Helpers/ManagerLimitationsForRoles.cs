using Biblioteca.Domain.Models.Enums;
using System.Security.Claims;

namespace Biblioteca.API.Helpers
{
    public static class ManagerLimitationsForRoles
    {
        public static bool CheckAccessResourceForRol(ClaimsPrincipal User, Guid idUserLoggedin)
        {
            Guid userIdAuthorized = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return (!User.IsInRole(nameof(Rol.Administrador)) ||
                !userIdAuthorized.Equals(idUserLoggedin));
        }
    }
}
