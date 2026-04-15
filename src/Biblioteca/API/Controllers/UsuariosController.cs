using Biblioteca.API.Helpers;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Usuarios;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Domain.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Biblioteca.API.Controllers
{
    /// <summary>
    /// Gestiona todas las operaciones relacionadas con el Usuario.
    /// </summary>
    /// <param name="_service"></param>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController (IUsuarioService _service) : ControllerBase
    {
        /// <summary>
        ///     Lista todos los usuarios existentes.
        /// </summary>
        /// <param name="pagination">
        ///     Representa los datos para aplicar paginacion.
        /// </param>
        /// <response code="200">Autores mostrados correctamente</response>
        /// <response code="400">Datos incorrectos</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<DetailUsuarioDTO>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<ActionResult<PagedResult<DetailUsuarioDTO>>> GetUsuarios(
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.GetUsuariosAsync(pagination);

            return Ok(result);
        }


        /// <summary>
        ///     Busca un usuario con el Id que pase por la url.
        /// </summary>
        /// <param name="id">
        ///     Valor ID para buscar al usuario.
        /// </param>
        /// <response code="200">Autor encontrado y mostrado</response>
        /// <response code="400">Datos incorrectos</response>
        /// <response code="404">Autor no encontrado (no existe el ID en la BD)</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DetailUsuarioDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailUsuarioDTO>> GetUsuarioById(Guid id)
        {
            if (!ManagerLimitationsForRoles.CheckAccessResourceForRol(User, id)) return Forbid("No tienes Acceso a este recurso.");

            var userFound = await _service.GetUsuarioByIdAsync(id);

            return Ok(userFound);
        }

        /// <summary>
        ///     Lista todos los prestamos de un usuario.
        /// </summary>
        /// <param name="id">
        ///     Valor ID para identificar al usuario.
        /// </param>
        /// <param name="pagination">
        ///     Datos necesario para la paginacion de la consulta.
        /// </param>
        /// <response code="200">Autor encontrado y lista de prestamos consultada</response>
        /// <response code="400">Datos incorrectos</response>
        /// <response code="404">Autor no encontrado (no existe el ID en la BD)</response>
        [HttpGet("{id}/prestamos")]
        [ProducesResponseType(typeof(PagedResult<DetailPretamosUsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<DetailPretamosUsuarioDTO>>> GetPretamosUsuarioById(
            Guid id,
            [FromQuery] PaginationParams pagination)
        {
            if (!ManagerLimitationsForRoles.CheckAccessResourceForRol(User, id)) return Forbid("No cuentas con los permisos necesarios");

            var loansUser = await _service.GetPrestamosUsuarioByIdAsync(id, pagination);

            return Ok(loansUser);
        }

        /// <summary>
        ///     Crea un nuevo usuario al sistema.
        /// </summary>
        /// <param name="autorDTO">
        ///     Datos necesarios para crear el nuevo usuario.
        /// </param>
        /// <response code="201">Usuario correctamente creado</response>
        /// <response code="400">Datos incorrectos</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost]
        [ProducesResponseType(typeof(DetailUsuarioDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostUsuario([FromBody] CreateNewUsuarioDTO autorDTO)
        {
            var elementCreated = await _service.CreateUsuarioAsync(autorDTO);

            return CreatedAtAction(
                nameof(GetUsuarioById),
                new { id = elementCreated.UsuarioId },
                elementCreated
            );
        }

        /// <summary>
        ///     Actualiza un la informacion de un usuario
        /// </summary>
        /// <param name="id">
        ///     Valor ID para identificar al usuario.
        /// </param>
        /// <param name="autorDTO">
        ///     Datos que se actualizaran,
        /// </param>
        /// <response code="204">Usuario correctamente creado</response>
        /// <response code="400">Datos incorrectos</response>
        /// <response code="404">Usuaario no encontrado</response>
        /// <response code="409">Se quiere actualizar un recurso que no existe</response>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PatchUsuario(Guid id, [FromBody] UpdateUsuarioDTO autorDTO)
        {
            if (!ManagerLimitationsForRoles.CheckAccessResourceForRol(User, id)) return Forbid("No cuentas con los permisos necesarios");

            await _service.UpdateUsuarioAsync(id, autorDTO);

            return NoContent();
        }


        /// <summary>
        ///     Borra logicamente un autor especifico
        /// </summary>
        /// <param name="id">
        ///     Valor ID para identificar al usuario
        /// </param>
        /// <response code="204">Usuario correctamente borrado</response>
        /// <response code="400">Datos incorrectos</response>
        /// <response code="404">Usuaario no encontrado</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUsuario(Guid id)
        {
            await _service.DeleteUsuarioAsync(id);

            return NoContent();
        }

    }
}
