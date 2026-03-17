using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Libros;
using Biblioteca.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Controllers
{
    /// <summary>
    /// Gestiona todas las operaciones relacionadas con los libros.
    /// </summary>
    /// <param name="_service"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class LibrosController(ILibroService _service) : ControllerBase
    {
        /// <summary>
        ///     Lista todos los libros existentes (paginado).
        /// </summary>
        /// <param name="pagination">Parámetros de paginación.</param>
        /// <response code="200">Libros obtenidos correctamente.</response>
        /// <response code="400">Datos de consulta invalidos (ProblemDetails).</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<DetalleLibroDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<DetalleLibroDTO>>> GetLibros(
            [FromQuery] PaginationParams pagination
        )
        {
            var resultado = await _service.GetLibrosAsync(pagination);
            return Ok(resultado);
        }

        /// <summary>
        ///     Obtiene un libro por su Id.
        /// </summary>
        /// <param name="id">Id del libro.</param>
        /// <response code="200">Libro encontrado.</response>
        /// <response code="400">Id invalido (ProblemDetails).</response>
        /// <response code="404">Libro no encontrado (ProblemDetails).</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DetalleLibroDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetalleLibroDTO>> GetLibro(Guid id)
        {
            var librobuscado = await _service.GetLibroByIdAsync(id);
            return Ok(librobuscado);
        }

        /// <summary>
        ///     Crea un nuevo libro.
        /// </summary>
        /// <param name="libroDTO">Datos del libro a crear.</param>
        /// <response code="201">Libro creado (detalle devuelto).</response>
        /// <response code="400">Datos invalidos (ProblemDetails).</response>
        [HttpPost]
        [ProducesResponseType(typeof(DetalleLibroDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostLibro([FromBody] CreateNewLibroDTO libroDTO)
        {
            var elementoCreado = await _service.CreateNewLibroAsync(libroDTO);
            return CreatedAtAction(
                nameof(GetLibro),
                new { id = elementoCreado.LibroId },
                elementoCreado
            );
        }

        /// <summary>
        ///     Actualiza parcialmente un libro.
        /// </summary>
        /// <param name="id">Id del libro a actualizar.</param>
        /// <param name="libro">Datos a actualizar.</param>
        /// <response code="204">Actualizado correctamente (sin contenido).</response>
        /// <response code="400">Datos invalidos (ProblemDetails).</response>
        /// <response code="404">Libro no encontrado (ProblemDetails).</response>
        /// <response code="409">
        ///     Conflicto de negocio. Se intenta eliminar autores que en primera
        ///     instancia no estan relacionados con el libro encuestion (ProblemDetails).
        /// </response>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PatchLibro(Guid id, [FromBody] UpdateLibroDTO libro)
        {
            await _service.UpdateLibroAsync(id, libro);
            return NoContent();
        }

        /// <summary>
        ///     Elimina logicamente un libro.
        /// </summary>
        /// <param name="id">Id del libro a eliminar.</param>
        /// <response code="204">Eliminado correctamente (sin contenido).</response>
        /// <response code="400">Id invalido (ProblemDetails).</response>
        /// <response code="404">Libro no encontrado (ProblemDetails).</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLibro(Guid id)
        {
            await _service.DeleteLibroAsync(id);
            return NoContent();
        }
    }
}
