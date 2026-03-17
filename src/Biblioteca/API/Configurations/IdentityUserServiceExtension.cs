using Biblioteca.Domain.Models;
using Biblioteca.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.API.Configurations
{
    public static class IdentityUserServiceExtension
    {
        public static IServiceCollection AddIdentityUserConfig(this IServiceCollection services)
        {
            services.AddIdentity<AplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;

            })
            .AddEntityFrameworkStores<BibliotecaContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
