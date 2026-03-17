using Biblioteca.Aplication.DTOs.Auth;
using Biblioteca.Aplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Biblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService _service,
        IConfiguration _config
        ) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> PostLogin(
            [FromBody] LoginRequestDTO dto)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();
            TokenResultDTO result = await _service.LoginAsync(dto, ipAddress);

            ConfigurationCookieRefresh(result);

            return Ok(new LoginResponseDTO()
            {
                AccessToken = result.AccessToken
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> PostRegister(
            [FromBody] RegisterRequestDTO dto)
        {
            string clientOrigin = HttpContext.Request.Headers.Origin.ToString();

            RegisterResponseDTO result = await _service.AuthRegisterAsync(dto, clientOrigin!);

            var url = Url.Action(nameof(UsuarioController.GetUsuarioById), new { id = result.UsuarioId });

            return Created(url, null);

        }


        [HttpGet("refresh")]
        public async Task<IActionResult> GetRefreshToken()
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();

            string rToken = Request.Cookies["refreshToken"]!;

            if (rToken.Equals(string.Empty)) return Unauthorized();

            LoginResponseDTO result = await _service.RefreshTokenAsync(rToken, ipAddress!);

            return Ok(result);

        }

        [Authorize]
        public async Task<IActionResult> PostLogout()
        {
            // En caso de que el cliente no retorne por Cookie el refreshToken,
            // se dara como sesion teminada ya que el propio cliente no tiene
            // el token para refrescar el inicio de sesion.
            string ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();
            string? refreshTokenFromUser = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshTokenFromUser))
            {
                await _service.RevokeRefreshTokenAsync(refreshTokenFromUser, ipAddress);

                ConfigurationCookieRefresh(new TokenResultDTO() 
                {
                    RefreshToken = refreshTokenFromUser
                });
            }

            return Ok(
                new
                {
                    Success = true,
                    Message = "Sesion cerrada Correctamente"
                });
        }


        // Configura la cookie de respuesta de la peticion HTTP dependiendo de la accion del usuario.
        // Por defecto, ConfigurationCookieRefresh crea una cookie HttpOnly con el valor de
        // el refreshToken en un inicio de sesion ("/login").
        // Cuando queremos cerrar sesion (logout) es necesario destruir el refreshToken de la cookie del navegador.
        // Cambiamos el funcionamiento de la funcion con el parametro booleano por defecto "isDeleting".
        private void ConfigurationCookieRefresh(TokenResultDTO dto, bool isDeleting = false)
        {
            var secure = _config.GetValue<string>("Cookies:Secure");
            var sameSite = _config.GetValue<string>("Cookies:SameSite");
            var path = _config.GetValue<string>("Cookies:Path");

            if (string.IsNullOrEmpty(secure) ||
                string.IsNullOrEmpty(sameSite) ||
                string.IsNullOrEmpty(path)) throw new Exception("Informacion de entorno no configurada");
            // Lanzara una exception en caso de que no encuentre el valor de esas variables en el 
            // appsettings.Development.json mientras el entorno este declarado como en desarrollo.
            // En su defecto, tomara los valores que deben de ir en produccion. (appsettings)

            CookieOptions options = new()
            {
                HttpOnly = true,
                Secure = bool.Parse(secure),
                SameSite = Enum.Parse<SameSiteMode>(sameSite),
                Expires = DateTimeOffset.UtcNow.AddDays(dto.DiasParaExpirar),
                Path = path
            };

            if (!isDeleting)
            {
                HttpContext.Response.Cookies
                    .Append("refreshToken",
                    dto.RefreshToken,
                   options
                );
            }
            else
            {
                HttpContext.Response.Cookies.Delete("refreshToken", options);
            }
        }
    }
}
