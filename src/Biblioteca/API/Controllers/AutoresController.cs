using Biblioteca.Aplication.DTOs.Autores;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Controllers;

/// <summary>
/// Gestiona todas las operaciones relacionadas con los Autores
/// </summary>
/// <param name="_service"></param>
[ApiController]
[Route("api/[controller]")]
public class AutoresController(IAutorService _service) : ControllerBase
{
    /// <summary>
    ///     Lista todos los autores existentes (paginado).
    /// </summary>
    /// <param name="pagination">Parametros de paginacion.</param>
    /// <response code="200">Autores obtenidos correctamente.</response>
    /// <response code="400">Datos de consulta invalidos (ProblemDetails).</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DetailAutorDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<DetailAutorDTO>>> GetAutors(
        [FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAutorsAsync(pagination);
        return Ok(result);
    }

    /// <summary>
    ///     Obtiene un autor por su Id.
    /// </summary>
    /// <param name="id">Id del autor.</param>
    /// <response code="200">Autor encontrado.</response>
    /// <response code="400">Id invalido (ProblemDetails).</response>
    /// <response code="404">Autor no encontrado (ProblemDetails).</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DetailAutorDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetailAutorDTO?>> GetAutor(Guid id)
    {
        var autorFound = await _service.GetAutorByIdAsync(id);
        return Ok(autorFound);
    }

    /// <summary>
    ///     Crea un nuevo autor.
    /// </summary>
    /// <param name="autorDTO">Datos del autor a crear.</param>
    /// <response code="201">Autor creado correctamente.</response>
    /// <response code="400">Datos invalidos (ProblemDetails).</response>
    [HttpPost]
    [ProducesResponseType(typeof(DetailAutorDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAutor([FromBody] SaveAutorDTO autorDTO)
    {
        var elementCreated = await _service.SaveAutorAsync(autorDTO);

        return CreatedAtAction(
            nameof(GetAutor),
            new { id = elementCreated.AutorId },
            elementCreated
        );
    }

    /// <summary>
    ///     Actualiza la informacion de un autor.
    /// </summary>
    /// <param name="id">Id del autor.</param>
    /// <param name="autorDTO">Datos a actualizar.</param>
    /// <response code="204">Actualizado correctamente (sin contenido).</response>
    /// <response code="400">Datos invalidos (ProblemDetails).</response>
    /// <response code="404">Autor no encontrado (ProblemDetails).</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchAutor(Guid id, [FromBody] UpdateAutorDTO autorDTO)
    {
        await _service.UpdateAutorAsync(id, autorDTO);
        return NoContent();
    }

    /// <summary>
    ///     Elimina logicamente un autor.
    /// </summary>
    /// <param name="id">Id del autor.</param>
    /// <response code="204">Eliminado correctamente (sin contenido).</response>
    /// <response code="400">Id invalido (ProblemDetails).</response>
    /// <response code="404">Autor no encontrado (ProblemDetails).</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAutor(Guid id)
    {
        await _service.DeleteAutorAsync(id);
        return NoContent();
    }
}
