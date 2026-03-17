using Biblioteca.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Middlewares;


public class ExceptionMiddleware ( RequestDelegate _next )
{
    private readonly RequestDelegate _next = _next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex) 
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new ProblemDetails()
            {
                Type = ex.Type,
                Title = ex.Title,
                Status = ex.StatusCode,
                Detail = ex.Message,
                Instance = context.Request.Path
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new {
                Type = "https://httpstatuses.com/500",
                Title = "Error interno del servidor",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
                Instance = context.Request.Path
            });
        }
    }
}
