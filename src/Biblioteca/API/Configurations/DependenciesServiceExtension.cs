using Biblioteca.Aplication.Interfaces;
using Biblioteca.Aplication.Services;
namespace Biblioteca.API.Configurations
{
    public static class DependenciesServiceExtension
    {
        public static IServiceCollection AddDependencys(this IServiceCollection services) 
        {
            services.AddScoped<ILibroService, LibroService>();
            services.AddScoped<IAutorService, AutorService>();
            services.AddScoped<IPrestamoService, PrestamosService>();
            services.AddScoped<IEjemplarService, EjemplarService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddSingleton<ITokenService, TokenService>();

            return services;
        }
    }
}
