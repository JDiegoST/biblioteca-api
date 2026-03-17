using Biblioteca.Aplication.DTOs.Auth;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Domain.Exceptions;
using Biblioteca.Domain.Models;
using Biblioteca.Domain.Models.Auth;
using Biblioteca.Domain.Models.Enums;
using Biblioteca.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Principal;

namespace Biblioteca.Aplication.Services
{
    public class AuthService(
        BibliotecaContext _context,
        UserManager<AplicationUser> _userManager,
        //SignInManager<AplicationUser> _signInManager,
        ITokenService _tokenService
    ) : IAuthService
    {
        // Valor que hace referencia a la cantidad de dias que un token
        // de refresco se mantiene como activo
        public const short DIAS_A_EXPIRAR = 7;

        public async Task<TokenResultDTO> LoginAsync(LoginRequestDTO dto, string? ipAddress)
        {
            var identityUser = await _userManager.FindByEmailAsync(dto.Email) 
                ?? throw new UnauthorizedException($"Credenciales incorrectas.");

            var isSucces = await _userManager.CheckPasswordAsync(identityUser, dto.Password);
            if(!isSucces) throw new UnauthorizedException($"Credenciales incorrectas.");

            var userRol = await _userManager.GetRolesAsync(identityUser);
            //Console.WriteLine(userRol.ToString());

            string accessToken = _tokenService.GenerateAccessToken(new List<Claim>() {
                 new ("Email", $"{identityUser.Email}"),
                 new ("Role", $"{userRol.First()}"),
                 new ("NameIdentifier", $"{identityUser.Id}")
             });

            var tokenRefresh = await SaveRefreshTokenDB(identityUser.Id, ipAddress!);

            return new TokenResultDTO()
            {
                AccessToken = accessToken,
                RefreshToken = tokenRefresh,
                DiasParaExpirar = DIAS_A_EXPIRAR
            };
                
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            string hashRefreshTokenCliente = _tokenService.HashToken(refreshToken);

            RefreshToken refreshTokenFromDB = await _context.RefreshTokens
                .Where(rt => rt.TokenHash.Equals(hashRefreshTokenCliente))
                .FirstOrDefaultAsync() ??
                throw new UnauthorizedException("Recurso no disponible. Vuelve a iniciar sesion.");

            if (refreshTokenFromDB.ExpiresAt > DateTime.Now)
                refreshTokenFromDB.RevokedAt = DateTime.Now;

            if (refreshTokenFromDB.IsExpired)
                throw new UnauthorizedException("Sesion caducada. Vuelve a iniciar sesion.");

            AplicationUser identiyUser = await _userManager.FindByIdAsync(refreshTokenFromDB.IdentityUserId.ToString())
                ?? throw new Exception();

            var userRol = (await _userManager.GetRolesAsync(identiyUser)).FirstOrDefault();

            String newAccessToken = _tokenService.GenerateAccessToken( new List<Claim>() 
            {
                 new ("Email", $"{identiyUser.Email}"),
                 new ("Rol", $"{userRol}"),
                 new ("UserId", $"{identiyUser.Id}")
            });

            string newRefreshToken = await SaveRefreshTokenDB(identiyUser.Id, ipAddress);

            return new LoginResponseDTO()
            {
                AccessToken = newAccessToken
            };
        }

        public async Task<RegisterResponseDTO> AuthRegisterAsync(RegisterRequestDTO dto, string origin)
        {
            var user = await _userManager.FindByEmailAsync(dto.EmailAddress);
            
            if (user is not null) throw new ConflictException($"El correo: {dto.EmailAddress} ya esta registrado");


            var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var identityUser = new AplicationUser()
                {
                    Email = dto.EmailAddress,
                    UserName = dto.EmailAddress,
                    EmailConfirmed = true,
                };

                IdentityResult result = await _userManager.CreateAsync(identityUser, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new ConflictException($"No fue posible crear el usuario: {errors}");
                }

                var userRol = nameof(Rol.Lector);

                var p = await _userManager.AddToRoleAsync(identityUser, userRol);

                if ((string.IsNullOrWhiteSpace(dto.Apellido) ||
                  string.IsNullOrWhiteSpace(dto.Nombre)))
                {
                    await tx.CommitAsync();
                    return new RegisterResponseDTO()
                    {
                        Email = identityUser.Email,
                        Rol = userRol
                    };
                }

                var dataUser = new Usuario()
                {
                    UsuarioId = Guid.NewGuid(),
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    IdentityUserId = identityUser.Id,
                };

                await _context.Usuarios.AddAsync(dataUser);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new RegisterResponseDTO()
                {
                    NombreCompleto = dataUser.NombreCompleto,
                    Email = identityUser.Email,
                    Rol = userRol
                };
            }
            catch (Exception ex) 
            {
                await tx.RollbackAsync();
                throw new Exception("Ocurrio un error al intentar guardar el usuario.\n" + ex.Message);
            }
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, string ipAddress)
        {
            string hashRefreshToken = _tokenService.HashToken(refreshToken);

            RefreshToken? tokenInDataBase = await _context.RefreshTokens
                .Where(r => r.TokenHash.Equals(hashRefreshToken))
                .FirstOrDefaultAsync();

            if (tokenInDataBase is null) return;

            _context.RefreshTokens.Remove(tokenInDataBase);

            await _context.SaveChangesAsync();            
        }

        /// <summary>
        /// Se guardar el hash del refresh token en la base de datos para su seguridad.
        /// En caso de rotacion de token el registro actual apunta al anterior al ser cambiado.
        /// </summary>
        /// <param name="identityId">
        ///     Representa el ID del usuario quien consulta un refresh
        /// </param>
        /// <param name="ipOrigen">
        ///     Utilizado para el registro de acciones (logger) para ver desde donde se intento hacer la accion.
        /// </param>
        private async Task<string> SaveRefreshTokenDB(Guid identityId, string ipOrigen)
        {
            string refreshToken = _tokenService.GenerateRefreshToken();

            string hashRefresh = _tokenService.HashToken(refreshToken);

            var newRefreshToken = new RefreshToken()
            {
                Id = Guid.NewGuid(),
                ExpiresAt = DateTime.UtcNow.AddDays(DIAS_A_EXPIRAR),
                CreatedByIp = ipOrigen,
                IdentityUserId = identityId,
                TokenHash = hashRefresh,
            };

            RefreshToken? refreshTokenToUser = await _context.RefreshTokens
                .Where(rt => rt.IdentityUserId == identityId)
                .FirstOrDefaultAsync();

            if (refreshTokenToUser is not null) 
                newRefreshToken.ReplacedByTokenId = refreshTokenToUser.Id;

            await _context.RefreshTokens.AddAsync(newRefreshToken);

            await _context.SaveChangesAsync();

            return refreshToken;

        }
    }
}
