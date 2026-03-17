using Biblioteca.Domain.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Infrastructure.Identity.seeds
{
    public static class SetSeedRolesInit
    {
        public async static void SeedRolesInitAsync(this IServiceProvider _app)
        {
            using var scope = _app.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            string[] roles = { nameof(Rol.Lector), nameof(Rol.Administrador) };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
