using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuscadorController(IBuscador _service) : ControllerBase
    {
        /// <summary>
        ///     Buscara cualquier libro o autor de la DB que se parezca a lo buscado por el usuario.
        /// </summary>
        /// <param name="filters">
        ///     Obtiene la informacion de los parametro en la consulta para personalizarla    
        /// </param>
        /// <param name="pagination">
        ///     Aplica paginacion al resultado de la consulta.
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSearching(
            [FromQuery] FilterParams filters,
            [FromQuery] PaginationParams pagination )
        {
            var results = await _service.GetSearchingAsync(filters, pagination);
            return Ok(results);
        }
    }
}
