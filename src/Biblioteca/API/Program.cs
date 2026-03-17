using Biblioteca.API.Configurations;
using Biblioteca.API.Middlewares;
using Biblioteca.Infrastructure.Context;
using Biblioteca.Infrastructure.Identity.seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// ------------------- SwaggerGen -------------------
builder.Services.AddSwaggerDocs();

builder.Services.AddMySql<BibliotecaContext>(
    builder.Configuration.GetConnectionString("connect"),
    new MySqlServerVersion(
        new Version(8, 0, 30)
    )
);

builder.Services.AddIdentityUserConfig();

builder.Services.AddAuthenticationAndAuthorizationConfig(builder.Configuration);

// Dependencias
builder.Services.AddDependencys();

var app = builder.Build();

// Como se utiliza Identity User para el control la sesion, los roles totales del sistema se guardan 
// de manera global en la Base de dato (DB) para su posterior uso
//app.Services.SeedRolesInitAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<ExceptionMiddleware>();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
