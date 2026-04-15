using Biblioteca.Aplication.DTOs.Autores;
using Biblioteca.Aplication.DTOs.Buscador;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Libros;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Domain.Models.Enums;
using Biblioteca.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Biblioteca.Aplication.Services
{
    public class BuscadorService (BibliotecaContext _context): IBuscador
    {
        public async Task<BuscadorResponseDTO> GetSearchingAsync(FilterParams filters, PaginationParams pagination)
        {
            var queryLibros = _context.Libros
                .AsNoTracking()
                .OrderBy(l => l.Titulo).AsQueryable();

            if (!string.IsNullOrEmpty(filters.Q))
            {
                var patter = $"%{filters.Q}%";
                queryLibros = queryLibros.Where(l =>
                    EF.Functions.Like(l.Titulo.ToLower(), patter) ||
                    EF.Functions.Like(l.Isbn.ToLower(), patter)
                );
            }

            if (filters.Disponible != null)
            {
                if (filters.Disponible.Value) queryLibros = queryLibros.Where(l => l.Ejemplares.ToList().Where(e => e.Estado == EstadoPrestamo.Disponible).Any());
                else queryLibros = queryLibros.Where(l => l.Ejemplares.ToList().Where(e => e.Estado == EstadoPrestamo.Indisponible).Any());
            }

            var queryAutores = _context.Autores
                .AsNoTracking()
                .OrderBy(l => l.Apellido).AsQueryable();

            if (!string.IsNullOrEmpty(filters.Q))
            {
                var patter = $"%{filters.Q}%";
                queryAutores = queryAutores.Where(l => EF.Functions.Like(l.Nombre + "" + l.Apellido, patter));
            }

            var totalLibroItems = await queryLibros.CountAsync();

            var totalAutorItems = await queryAutores.CountAsync();

            var libros = await queryLibros
           .Skip((pagination.PageNumber - 1) * pagination.PageSize)
           .Take(pagination.PageSize)
           .Select(e => new DetalleLibroDTO()
           {
               LibroId = e.LibroId,
               Titulo = e.Titulo,
               RutaImagen = e.RutaImagen,
               DescripcionFisica = e.DescripcionFisica,
               Isbn = e.Isbn,
               AnioPublicacion = e.AnioPublicacion,
               Editorial = e.Editorial,
               Autores = e.AutoresLibros.Select(a => $"{a.Autor.Nombre} {a.Autor.Apellido}").ToList()
           }).ToListAsync();

            var autors = await queryAutores
           .Skip((pagination.PageNumber - 1) * pagination.PageSize)
           .Take(pagination.PageSize)
           .Select(a => new DetailAutorDTO(a.Nombre, a.Apellido)
           {
               AutorId = a.AutorId,
               Biography = a.Biografia,
               UrlImg = a.UrlImgAutor
           })
           .ToListAsync();

            return new BuscadorResponseDTO()
            {
                Libros = new PagedResult<DetalleLibroDTO>() {
                    TotalItems = totalLibroItems,
                    Items = libros,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                },
                Autores = new PagedResult<DetailAutorDTO>()
                {
                    TotalItems = totalAutorItems,
                    Items = autors,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                }
            };
        }
    }
}
