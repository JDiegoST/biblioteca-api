using Microsoft.EntityFrameworkCore;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Ejemplares;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Infrastructure.Context;
using Biblioteca.Aplication.DTOs.Libros;
using Biblioteca.Domain.Exceptions;
using Biblioteca.Domain.Models;
using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Aplication.Services
{
    public class EjemplarService(BibliotecaContext _context) : IEjemplarService
    {
        /// <summary>
        ///     Retorna la lista completa de todos los elementos (Ejemplares) existentes en la base de datos.
        ///     Ademas, aplica paginacion a todos los datos para optimizar el flujo de informacion.
        /// </summary>
        /// <param name="pagination">
        ///     Representa DTO necesario para aplicar una correcta paginacion a el resultado de la consulta
        /// </param>
        public async Task<PagedResult<DetailEjemplarDTO>> GetAllEjemplaresAsync(PaginationParams pagination)
        {
            var query = _context.Ejemplares
                .AsNoTracking()
                .OrderBy(e => e.CodigoInventario);

            var totalItems = await query.CountAsync();

            var ejemplares = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(e => new DetailEjemplarDTO()
                {
                    EjemplarId = e.EjemplarId,
                    CodigoInventario = e.CodigoInventario,
                    Estado = e.Estado,
                    Libro = new SummaryLibroResumenDTO()
                    {
                        Titulo = e.Libro.Titulo,
                    },
                }).ToListAsync();

            return new PagedResult<DetailEjemplarDTO>()
            {
                TotalItems = totalItems,
                Items = ejemplares,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        /// <summary>
        ///     Encuentra y retorna el elemento especifico a partir de el ID
        /// </summary>
        /// <param name="id">
        ///     Representa el valor GUID para encontrar el elemento buscado
        /// </param>
        /// <returns></returns>
        public async Task<DetailEjemplarDTO?> GetEjemplarByIdAsync(Guid id)
        {
            return await _context.Ejemplares.AsNoTracking().Select( e => new DetailEjemplarDTO()
            {
                EjemplarId = e.EjemplarId,
                CodigoInventario = e.CodigoInventario,
                Estado = e.Estado,
                Libro = new SummaryLibroResumenDTO()
                {
                    Titulo = e.Libro.Titulo,
                }
            } ).FirstOrDefaultAsync() ?? throw new NotFoundException($"Ejemplar con Id: '{id}' no encontrado.");
        }

        /// <summary>
        ///     Verifica y retorna un valor booleano en caso de que el ejemplar buscado a partir de el parametro ID
        ///     se encuentra actualmente DISPONIBLE para ser prestado.
        /// </summary>
        /// <param name="ejemplarId">
        ///     Representa el Id que tiene el ejemplar buscado.
        /// </param>
        /// <returns></returns>
        public async Task<CheckAvailabilityEjemplar> FastCheckEjemplarAvailableAsync(Guid ejemplarId)
        {
            var loanSearched = await _context.Ejemplares
                .Where(e => e.EjemplarId == ejemplarId)
                .Select(e => e.Estado)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException($"Ejemplar id: '{ejemplarId}' no encontrado.");

            return new CheckAvailabilityEjemplar() {
                Disponible = loanSearched == EstadoPrestamo.Disponible
            };
        }

        /// <summary>
        ///     Verifica si un libro especifico tiene uno o varios ejemplares disponibles para
        ///     que alguno de ellos sea prestado.
        /// </summary>
        /// <param name="libroId">
        ///     Representa el ID que tiene un libro especifico en la BD.
        /// </param>
        /// <returns></returns>
        public async Task<CheckListEjemplaresAvailable> CheckEjemplaresAvailableByLibroIdAsync(Guid libroId)
        {
            var ejemplaresBuscados = await _context.Ejemplares
                .AsNoTracking()
                .Where(e => e.LibroId == libroId && e.Estado == EstadoPrestamo.Disponible)
                .Select(e => new DetailEjemplarDTO()
                {
                    EjemplarId = e.EjemplarId,
                    CodigoInventario = e.CodigoInventario,
                    Estado = e.Estado,
                    Libro = new SummaryLibroResumenDTO()
                    {
                        Titulo = e.Libro.Titulo,
                    }
                }).ToListAsync();

            return new CheckListEjemplaresAvailable()
            {
                EjemplaresDisponibles = ejemplaresBuscados
            };
        }
        
        /// <summary>
        ///     Inserta un nuevo ejemplar en la base de datos a partir de los datos enviados 
        ///     por el cliente.
        /// </summary>
        /// <param name="ejemplar">
        ///     DTO que permute obtener la informacion necesaria para crear un nuevo ejemplar.
        /// </param>
        /// <returns></returns>
        public async Task<DetailEjemplarDTO> CreateEjemplarAsync(CreateNewEjemplarDTO ejemplar)
        {
            var refLibro = await _context.Libros.FindAsync(ejemplar.LibroId)
                ?? throw new NotFoundException($"El libro con Id: '{ejemplar.LibroId}' no existe.");

            var newElement = new Ejemplar()
            {
                EjemplarId = Guid.NewGuid(),
                LibroId = ejemplar.LibroId,
                CodigoInventario = ejemplar.CodigoInventario,
                Estado = ejemplar.Estado,
            };

            await _context.AddAsync(newElement);

            await _context.SaveChangesAsync();

            return new DetailEjemplarDTO()
            {
                EjemplarId = newElement.EjemplarId,
                CodigoInventario = newElement.CodigoInventario,
                Estado = newElement.Estado,
                Libro = new SummaryLibroResumenDTO()
                {
                    Titulo = refLibro.Titulo,
                },
            };
        }

        /// <summary>
        ///     Permite actualizar un ejemplar especifico a partir de el ID.
        /// </summary>
        /// <param name="id">
        ///     Representa el valor GUID para buscar el ejemplar a actualizar en la base de datos.
        /// </param>
        /// <param name="ejemplar">
        ///     Representa el DTO que contiene aquellos datos que se quieran
        ///     actualizar del elemento especifico.
        /// </param>
        /// <returns></returns>
        public async Task UpdateEjemplarAsync(Guid id, UpdateEjemplarDTO ejemplar)
        {
            var elementFound = await _context.Ejemplares.FindAsync(id)
                ?? throw new NotFoundException($"El ejemplar con Id: '{id}' no existe.");

            if (ejemplar.LibroId.HasValue)
            {
                var l = await _context.Libros.FindAsync(ejemplar.LibroId)
                    ?? throw new NotFoundException($"El libro con id: '{ejemplar.LibroId}' del ejemplar al que haces referencia, no existe.");

                elementFound.LibroId = l.LibroId;
            }

            if (ejemplar.CodigoInventario != null)
            {
                elementFound.CodigoInventario = ejemplar.CodigoInventario;
            }

            await _context.SaveChangesAsync();
        }


        /// <summary>
        ///     Hace un borrado logico de este elemento en la base de datos (SOFT DELETE).
        /// </summary>
        /// <param name="id">
        ///     Representa el valor GUID para buscar el elmento especifico que sera borrado.
        /// </param>
        /// <returns></returns>
        public async Task DeleteEjemplarAsync(Guid id)
        {
            var elementFound = await _context.Ejemplares.FindAsync(id)
                ?? throw new NotFoundException($"El ejemplar id: '{id}' no existe.");

            _context.Remove(elementFound);
            await _context.SaveChangesAsync();
        }
    }
}
