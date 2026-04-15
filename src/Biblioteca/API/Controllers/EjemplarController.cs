using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Ejemplares;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Domain.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Controllers
{
    /// <summary>
    /// Gestiona todas las operaciones relacionas con los Ejemplares
    /// </summary>
    /// <param name="_service"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class EjemplarController(IEjemplarService _service) : ControllerBase
    {
        /// <summary>
        ///     Lista todos los ejemplares existentes (paginado).
        /// </summary>
        /// <param name="pagination">Parametros de paginacion.</param>
        /// <response code="200">Ejemplares obtenidos correctamente.</response>
        /// <response code="400">Datos de consulta invalidos (ProblemDetails).</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<DetailEjemplarDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<DetailEjemplarDTO>>> GetAllEjemplares(
            [FromQuery] PaginationParams pagination)
        {
            var ejemplares = await _service.GetAllEjemplaresAsync(pagination);
            return Ok(ejemplares);
        }

        /// <summary>
        ///     Obtiene un ejemplar por su Id.
        /// </summary>
        /// <param name="id">Id del ejemplar.</param>
        /// <response code="200">Ejemplar encontrado.</response>
        /// <response code="400">Id invalido (ProblemDetails).</response>
        /// <response code="404">Ejemplar no encontrado (ProblemDetails).</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DetailEjemplarDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEjemplarById(Guid id)
        {
            var ejemplar = await _service.GetEjemplarByIdAsync(id);
            return Ok(ejemplar);
        }

        /// <summary>
        ///     Crea un nuevo ejemplar.
        /// </summary>
        /// <param name="ejemplarDTO">Datos del ejemplar a crear.</param>
        /// <response code="201">Ejemplar creado correctamente.</response>
        /// <response code="400">Datos invalidos (ProblemDetails).</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost]
        [ProducesResponseType(typeof(DetailEjemplarDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostEjemplar([FromBody] CreateNewEjemplarDTO ejemplarDTO)
        {
            var ejemplar = await _service.CreateEjemplarAsync(ejemplarDTO);
            return CreatedAtAction(
                nameof(GetEjemplarById),
                new { id = ejemplar.EjemplarId },
                ejemplar
            );
        }

        /// <summary>
        ///     Actualiza parcialmente un ejemplar.
        /// </summary>
        /// <param name="id">Id del ejemplar.</param>
        /// <param name="ejemplar">Datos a actualizar.</param>
        /// <response code="204">Actualizado correctamente (sin contenido).</response>
        /// <response code="400">Datos invalidos (ProblemDetails).</response>
        /// <response code="404">Ejemplar no encontrado (ProblemDetails).</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchEjemplar(Guid id, [FromBody] UpdateEjemplarDTO ejemplar)
        {
            await _service.UpdateEjemplarAsync(id, ejemplar);
            return NoContent();
        }

        /// <summary>
        ///     Elimina logicamente un ejemplar.
        /// </summary>
        /// <param name="id">Id del ejemplar.</param>
        /// <response code="204">Eliminado correctamente (sin contenido).</response>
        /// <response code="400">Id invalido (ProblemDetails).</response>
        /// <response code="404">Ejemplar no encontrado (ProblemDetails).</response>
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEjemplar(Guid id)
        {
            await _service.DeleteEjemplarAsync(id);
            return NoContent();
        }
    }
}
