using Microsoft.OpenApi.Models;

namespace Biblioteca.API.Configurations
{
    public static class SwaggerServiceExtension
    {
        public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Biblioteca API",
                    Version = "v1",
                    Description = "API para gestión de biblioteca"
                });

                // Incluir XML comments
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

                // JWT Bearer auth (opcional)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Usa 'Bearer {token}'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                  {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                    Array.Empty<string>()
                  }
                });
            });

            return services;
        }
    }
}
