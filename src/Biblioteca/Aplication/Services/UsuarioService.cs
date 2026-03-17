using Biblioteca.Aplication.DTOs.Auth;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Direcciones;
using Biblioteca.Aplication.DTOs.Ejemplares;
using Biblioteca.Aplication.DTOs.Libros;
using Biblioteca.Aplication.DTOs.Prestamos;
using Biblioteca.Aplication.DTOs.Usuarios;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Domain.Exceptions;
using Biblioteca.Domain.Models;
using Biblioteca.Domain.Models.Enums;
using Biblioteca.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Biblioteca.Aplication.Services
{
    public class UsuarioService(
        BibliotecaContext _context,
        UserManager<AplicationUser> _userManager
        ) : IUsuarioService
    {

        /// <summary>
        ///     Obtiene todos los los usuarios registrados en el sistema y los 
        ///     envia al cliente aplicando paginacion a los datos.
        /// </summary>
        /// <param name="pagination">
        ///      Este parametro  en un objeto que representa
        ///     el DTO correpondiente para aplicar paginacion a la consulta.
        /// </param>
        /// <returns>
        ///     Retorna PagedResul, el cual contiene la lista de los usuarios registrados
        ///     y datos esenciales para aplicar paginacion.
        /// </returns>
        public async Task<PagedResult<DetailUsuarioDTO>> GetUsuariosAsync(PaginationParams pagination)
        {
            var query = _context.Usuarios
           .AsNoTracking()
           .OrderBy(p => p.Apellido)
               .ThenBy(p => p.Nombre);

            var totalItems = await query.CountAsync();

            var prestamos = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(u => new DetailUsuarioDTO
                {
                    UsuarioId = u.UsuarioId,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    //Rol = u.Rol,
                }
            ).ToListAsync();

            return new PagedResult<DetailUsuarioDTO>()
            {
                TotalItems = totalItems,
                Items = prestamos,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
            };
        }

        /// <summary>
        ///     Busca un usuario especifico a partir de un ID
        /// </summary>
        /// <param name="id">
        ///     Representa el valor GUID del ID para encontrarlo en la DB.
        /// </param>
        /// <returns>
        ///     Retorna un DTO con la informacion del registro buscado en caso de que exista.
        /// </returns>
        public async Task<DetailUsuarioDTO?> GetUsuarioByIdAsync(Guid id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.UsuarioId == id)
                .Select(u => new DetailUsuarioDTO
                {
                    UsuarioId = u.UsuarioId,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                   // Rol = u.Rol,
                    Direccion = u.Direccion == null
                        ? null
                        : new DetailDomicilioDTO
                        {
                            Pais = u.Direccion.Pais,
                            Estado = u.Direccion.Estado,
                            CodigoPostal = u.Direccion.CodigoPostal,
                            Municipio = u.Direccion.Municipio,
                            Colonia = u.Direccion.Colonia,
                            Calle = u.Direccion.Calle,
                            NumeroInterior = u.Direccion.NumeroInterior,
                            NumeroExterior = u.Direccion.NumeroExterior
                        }.ToString(),
                })
                .FirstOrDefaultAsync();

            if (usuario is null)
                throw new NotFoundException($"Usuario id '{id}' no encontrado.");

            return usuario;
        }

        /// <summary>
        ///     Obtiene todos los prestamos hechos por un usuario especifico.
        /// </summary>
        /// <param name="userId">
        ///     Valor GUID ID que representa el registro buscado
        /// </param>
        /// <param name="pagination">
        ///     Contiene los datos necesario del cliente para aplicar paginacion a la consulta.
        /// </param>
        /// <returns>
        ///     retorna un PagedResult que contiene los lista de los prestamos del usuario
        ///     ademas de datos necesario para la paginacion.
        /// </returns>
        public async Task<PagedResult<DetailPrestamoDTO>> GetPrestamosUsuarioByIdAsync(Guid userId, PaginationParams pagination)
        {
            var userIsHere = await _context.Usuarios.FindAsync(userId)
                ?? throw new NotFoundException($"Usuario id: '{userId}' no encontrado.");

            var query = _context.Prestamos
                .AsNoTracking()
                .Where(u => u.UsuarioId == userId)
                .OrderBy(p => p.FechaVencimiento);

            var totalItems = await query.CountAsync();

            var prestamosUser = await query.Select(p => new DetailPrestamoDTO()
            {
                Usuario = new SummaryUsuarioDTO()
                {
                    NombreCompleto = p.Usuario.NombreCompleto,
                    UsuarioId = userId,
                    //Rol = p.Usuario.Rol,
                },
                PrestamoId = p.PrestamoId,
                FechaPrestamo = p.FechaPrestamo,
                FechaVencimiento = p.FechaVencimiento,
                FechaDevolucion = p.FechaDevolucion,
                Ejemplar = new DetailEjemplarDTO()
                {
                    EjemplarId = p.EjemplarId,
                    CodigoInventario = p.Ejemplar.CodigoInventario,
                    Libro = new SummaryLibroResumenDTO()
                    {
                        Titulo = p.Ejemplar.Libro.Titulo,
                    }
                }
                
            }).ToListAsync();

            return new PagedResult<DetailPrestamoDTO>()
            {
                TotalItems = totalItems,
                Items = prestamosUser,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
            };
        }

        /// <summary>
        ///     Registra un nuevo usuario.
        /// </summary>
        /// <param name="dto">
        ///     Representa el DTO que contiene todos los datos necesarios para registrar un usuario nuevo.
        /// </param>
        /// <returns>
        ///     Retorna un DTO con la informacion del recurso creado.
        /// </returns>
        public async Task<DetailUsuarioDTO> CreateUsuarioAsync(CreateNewUsuarioDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.EmailAddress);

            if (user is not null) throw new ConflictException($"El correo: {dto.EmailAddress} ya esta registrado");


            var tx = await _context.Database.BeginTransactionAsync();

            var identityUser = new AplicationUser()
            {
                Email = dto.EmailAddress,
                UserName = dto.EmailAddress,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ConflictException($"No fue posible crear el usuario: {errors}");
            }

            var dataUser = new Usuario()
            {
                UsuarioId = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                IdentityUserId = identityUser.Id,
            };

            if (dto.Direccion is not null)
                dataUser.Direccion = GetDireccion(dto.Direccion, dataUser.UsuarioId);

            var userRol = nameof(Rol.Lector);
            if (dto.Rol is not null)
            {
                userRol = nameof(dto.Rol);
            }

            var p = await _userManager.AddToRoleAsync(identityUser, userRol);

            await _context.Usuarios.AddAsync(dataUser);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new DetailUsuarioDTO()
            {
                UsuarioId = dataUser.UsuarioId,
                Nombre = dataUser.NombreCompleto,
                Email = identityUser.Email,
                Rol = userRol
            };
        }

        /// <summary>
        ///     Registra una direccion a un usuario espcifico.
        /// </summary>
        /// <param name="usuarioId">
        ///     Representa el valor GUID ID del usuario al que se le asigna un domicilio.
        /// </param>
        /// <param name="dto">
        ///     Representa el DTO que contiene todos los datos necesarios para registrar un domicilio de un usuario.
        /// </param>
        /// <returns>
        ///     Retorna un DTO con la informacion de la direccion 
        /// </returns>
        public async Task<DetailDomicilioDTO> AsingDomicilioToUser(Guid usuarioId, CreateDireccionDTO dto)
        {
            var direccion = GetDireccion(dto, usuarioId);

            await _context.AddAsync(direccion);

            await _context.SaveChangesAsync();

            return new DetailDomicilioDTO()
            {
                Pais = direccion.Pais,
                Estado = direccion.Estado,
                CodigoPostal = direccion.CodigoPostal,
                Municipio = direccion.Municipio,
                Colonia = direccion.Colonia,
                Calle = direccion.Calle,
                NumeroInterior = direccion.NumeroInterior,
                NumeroExterior = direccion.NumeroExterior,
            };
        }

        /// <summary>
        ///     Actualiza la informacion de un usuario especifico
        /// </summary>
        /// <param name="id">
        ///     Representa el valor GUID ID a buscar.
        /// </param>
        /// <param name="dto">
        ///     DTO que contiene la informacion que se desea actualizar.
        /// </param>
        public async Task UpdateUsuarioAsync(Guid id, UpdateUsuarioDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == id)
                ?? throw new NotFoundException($"Usuario id '{id}' no encontrado.");

            
            if (dto.Nombre is not null)
                usuario.Nombre = dto.Nombre;

            if (dto.Apellido is not null)
                usuario.Apellido = dto.Apellido;

            //if (dto.Rol is not null)
            //    usuario.Rol = dto.Rol;


            if (dto.NewDireccion is not null)
            {
                var direccion = await _context.Direcciones
                    .FirstOrDefaultAsync(d => d.UsuarioId == id);

                if (direccion is null)
                {
                    throw new ConflictException(
                        $"El usuario id '{id}' no tiene una dirección registrada que modificar."
                    );
                }
                else
                {
                    MapDireccion(dto.NewDireccion, direccion);
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///     Elimina logicamente un usuario de la base de datos.
        /// </summary>
        /// <param name="id">
        ///     Representa el valor GUID ID del usuario que se quiere eliminar
        /// </param>
        public async Task DeleteUsuarioAsync(Guid id)
        {
            var userFound = await _context.Usuarios.FindAsync(id) 
                ?? throw new NotFoundException($"Usuario id: '{id}' no existe o ya fue eliminado.");

            _context.Remove(userFound);
            
            // Al aplicar Soft y Direccion depender completamente de la existencia de 
            // un usuario. Necesito borrar la direccion si el usuario es borrado para
            // mantener la consistencia de los datos en la base de datos
            var direccion = await _context.Direcciones.FindAsync(id);

            if (direccion is not null) _context.Remove(direccion);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///     Convierte de un CreateDireccionDTO a un modelo Direccion.
        /// </summary>
        /// <param name="domicilio">
        ///     Representa el DTO recivido que contiene los datos para crear un nuevo usuario.
        /// </param>
        /// <param name="usuarioID">
        ///     ID GUID del usuario que funge el valor del ID de la nueva dirrecion
        ///     (La relacion logica entre ambos Direccion y Usuario es 1:1 
        ///     , ya que Direccion es totalmente dependiente si existe o no el usuario
        ///     el ID del usuario se vuelve el ID de la direccion creada).
        /// </param>
        /// <returns></returns>
        private static Direccion GetDireccion(CreateDireccionDTO domicilio, Guid usuarioID)
        {
            return new Direccion()
            {
                UsuarioId = usuarioID,
                Pais = domicilio.Pais,
                Estado = domicilio.Estado,
                CodigoPostal = domicilio.CodigoPostal,
                Municipio = domicilio.Municipio,
                Colonia = domicilio.Colonia,
                Calle = domicilio.Calle,
                NumeroInterior = domicilio.NumeroInterior,
                NumeroExterior = domicilio.NumeroExterior
            };
        }

        /// <summary>
        ///     Mapea la informacion del recurso al target (del DTO para actualizar el registro
        ///     al objeto del modelo Direccion.
        /// </summary>
        /// <param name="source">
        ///     DTO que contiene los datos que se mapearan.
        /// </param>
        /// <param name="target">
        ///     Direccion a la que se mapearan los valores del DTO.
        /// </param>
        private static void MapDireccion(UpdateDomicilioDTO source, Direccion target)
        {
            if (source.Pais is not null)
                target.Pais = source.Pais;

            if (source.Estado is not null)
                target.Estado = source.Estado;

            if (source.CodigoPostal is not null)
                target.CodigoPostal = source.CodigoPostal.Value;

            if (source.Municipio is not null)
                target.Municipio = source.Municipio;

            if (source.Colonia is not null)
                target.Colonia = source.Colonia;

            if (source.Calle is not null)
                target.Calle = source.Calle;

            if (source.NumeroInterior is not null)
                target.NumeroInterior = source.NumeroInterior;

            if (source.NumeroExterior is not null)
                target.NumeroExterior = source.NumeroExterior;
        }
    }
}
