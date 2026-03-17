using Biblioteca.API.Helpers;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Prestamos;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Domain.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Controllers
{
    /// <summary>
    /// Gestiona las operaciones relacionadas con los préstamos de ejemplares.
    /// </summary>
    [Authorize(Roles = $"{nameof(Rol.Administrador)}, {nameof(Rol.Lector)}")]
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController(IPrestamoService _service) : ControllerBase
    {
        private readonly IPrestamoService _service = _service;

        /// <summary>
        /// Obtiene la lista paginada de todos los préstamos.
        /// </summary>
        /// <param name="pagination">Parámetros de paginación.</param>
        /// <response code="200">Lista Obtenida</response>
        /// <response code="400">Datos incorrectos (ProblemDetails)</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<DetailPrestamoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<DetailPrestamoDTO>>> GetAllLoans(
            [FromQuery] PaginationParams pagination)
        {
            var results = await _service.GetAllLoansAsync(pagination);
            return Ok(results);
        }

        /// <summary>
        /// Obtiene el detalle de un préstamo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del préstamo.</param>
        /// <response code="200">Prestamo Obtenido.</response>
        /// <response code="400">Datos incorrectos (ProblemDetails)</response>
        /// <response code="404">Pretamo no encontrado (ProblemDetails)</response>
        
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DetailPrestamoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailPrestamoDTO>> GetLoanById(Guid id)
        {
            if (!ManagerLimitationsForRoles.CheckAccessResourceForRol(User, id)) return Forbid("No cuentas con los permisos necesarios");

            var result = await _service.GetLoanByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Registra un nuevo préstamo de un ejemplar.
        /// </summary>
        /// <param name="newLoan">Datos necesarios para crear el préstamo.</param>
        /// <response code="201">Prestamo Creado.</response>
        /// <response code="400">Datos incorrectos (ProblemDetails)</response>
        /// <response code="404">Usuario o Ejemplar no encontrado (ProblemDetails)</response>
        /// <response code="409">Ejemplar no disponible (ProblemDetails)</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost]
        [ProducesResponseType(typeof(DetailPrestamoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PostCreatePrestamo([FromBody] CreateNewPrestamoDTO newLoan)
        {
            var elementCreated = await _service.CreateNewLoanAsync(newLoan);

            return CreatedAtAction(
                nameof(GetLoanById),
                new { id = elementCreated.PrestamoId },
                elementCreated
            );
        }

        /// <summary>
        /// Marca un préstamo como devuelto.
        /// </summary>
        /// <param name="id">Identificador del préstamo.</param>
        /// <response code="204">Prestamo devuelto</response>
        /// <response code="400">Datos incorrectos (ProblemDetails)</response>
        /// <response code="404">Prestamo no encontrado (ProblemDetails)</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost("{id:guid}/devolver")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDevolverPrestamo(Guid id)
        {
            await _service.GoBackLoanAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Extiende la fecha de devolución de un préstamo.
        /// </summary>
        /// <param name="id">Identificador del préstamo.</param>
        /// <param name="updateLoan">Datos de actualización del préstamo.</param>
        /// <response code="204">Limite del prestamo extendido.</response>
        /// <response code="400">Datos incorrectos (ProblemDetails)</response>
        /// <response code="404">Prestamo no encotrado(ProblemDetails)</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPatch("{id:guid}/extender")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchExtenderPrestamo(Guid id, [FromBody] UpdatePrestamoDTO updateLoan)
        {
            await _service.UpdateLoanAsync(id, updateLoan);
            return NoContent();
        }
    }
}
