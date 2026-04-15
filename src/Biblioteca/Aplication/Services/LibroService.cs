using Microsoft.EntityFrameworkCore;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Libros;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Infrastructure.Context;
using Biblioteca.Domain.Models;
using Biblioteca.Domain.Exceptions;
using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Aplication.Services;
public class LibroService(BibliotecaContext _context) : ILibroService
{
    /// <summary>
    ///     Obtiene todos los libros registrados en la DB.
    ///     Aplica paginacion a los resultados.
    /// </summary>
    /// <param name="pagination">
    ///     Contiene la informacion necesaria para aplicar paginacion a los resultados.
    /// </param>
    /// <returns>
    ///     Retorna un objeto PagedResutl que ademas de la cantidad y los elementos encontrados,
    ///     contiene informacion importante acerca de la paginacion.
    /// </returns>
    public async Task<PagedResult<DetalleLibroDTO>> GetLibrosAsync(FilterParams filters, PaginationParams pagination)
    {

        var query = _context.Libros
            .AsNoTracking()
            .OrderBy(l => l.Titulo).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Q))
        {
            query = query.Where(l =>
                EF.Functions.Like(l.Titulo, $"%{filters.Q}%"));
        }

        if (filters.Disponible.HasValue)
        {
            query = query.Where(l =>
                l.Ejemplares.ToList().Where(e => e.Estado == EstadoPrestamo.Disponible).Any());
        }

        var totalItems = await query.CountAsync();

        var libros = await query
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


        return new PagedResult<DetalleLibroDTO>()
        {
            TotalItems = totalItems,
            Items = libros,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
        };
    }

    /// <summary>
    ///     Obtiene un libro especifico a partir del ID
    /// </summary>
    /// <param name="id">
    ///     Valor GUID ID para burcarlo en la DB
    /// </param>
    /// <returns>
    ///     Retorna un DTO con la informacion de el libro encontrado en caso de que
    ///     exista en la base de datos.
    /// </returns>
    public async Task<DetalleLibroDTO?> GetLibroByIdAsync(Guid id)
    {
        var libro = await _context.Libros.AsNoTracking()
            .Include(l => l.AutoresLibros)
            .ThenInclude(al => al.Autor)
            .FirstOrDefaultAsync(l => l.LibroId == id)
            ?? throw new NotFoundException($"El libro id: '{id}', no existe.");

        var libroDto = new DetalleLibroDTO
        {
            LibroId = libro.LibroId,
            Titulo = libro.Titulo,
            RutaImagen = libro.RutaImagen,
            DescripcionFisica = libro.DescripcionFisica,
            Isbn = libro.Isbn,
            AnioPublicacion = libro.AnioPublicacion,
            Editorial = libro.Editorial,
            Autores = libro.AutoresLibros.Select(a => $"{a.Autor.Nombre} {a.Autor.Apellido}").ToList(),
        };

        return libroDto;
    }

    /// <summary>
    ///     Crea un nuevo libro y lo guarda en la base de datos.
    /// </summary>
    /// <param name="newLibro">
    ///     DTO que contiene toda la informacion necesario para crear un nuevo 
    ///     registro de la entidad LIBRO.
    /// </param>
    /// <returns>
    ///     Retorna un DTO de respuesta con la informacion de el nuevo recurso (libro)
    ///     que se acaba de crear en el sistema.
    /// </returns>
    public async Task<DetalleLibroDTO> CreateNewLibroAsync(CreateNewLibroDTO newLibro)
    {
        var libro = new Libro()
        {
            LibroId = Guid.NewGuid(),
            Titulo = newLibro.Titulo,
            Isbn = newLibro.Isbn,
            DescripcionFisica = newLibro.DescripcionFisica,
            RutaImagen = newLibro.RutaImagen,
            AnioPublicacion = newLibro.AnioPublicacion,
            Editorial = newLibro.Editorial
        };

        if (newLibro.Autores.Count > 0)
        {
            var autores = await _context.Autores.Where(a => newLibro.Autores.Contains(a.AutorId)).ToListAsync();

            if (newLibro.Autores.Count != autores.Count) throw new BusinesException("Uno o más autores no estan registrados.");

            libro.AutoresLibros = [.. autores.Select(al => new AutorLibro
            {
                AutorId = al.AutorId,
                LibroId = libro.LibroId,
            }) ];
        }

        await _context.AddAsync(libro);
        await _context.SaveChangesAsync();

        return new DetalleLibroDTO
        {
            LibroId = libro.LibroId,
            Titulo = libro.Titulo,
            Isbn = libro.Isbn,
            Editorial = libro.Editorial,
            AnioPublicacion = libro.AnioPublicacion,
            DescripcionFisica = libro.DescripcionFisica,
            RutaImagen = libro.RutaImagen,
            Autores = [.. libro.AutoresLibros.Select(a => $"{a.Autor.Nombre} {a.Autor.Apellido}") ]
        };
    }

    /// <summary>
    ///     Actualiza la informacion de un registro (libro) especifico a partir de su ID
    /// </summary>
    /// <param name="id">
    ///     Valor GUID ID para buscar el elemento al que se le quiere modificar la informacion.
    /// </param>
    /// <param name="dto">
    ///     DTO con la informacion que se quiere modificar del registro actual.
    /// </param>
    public async Task UpdateLibroAsync(Guid id, UpdateLibroDTO dto)
    {
        var libro = await _context.Libros
            .FirstOrDefaultAsync(l => l.LibroId == id)
            ?? throw new NotFoundException($"Libro con Id '{id}' no existe.");

        if (dto.Titulo is not null) libro.Titulo = dto.Titulo;
        if (dto.RutaImagen is not null) libro.RutaImagen = dto.RutaImagen;
        if (dto.DescripcionFisica is not null) libro.DescripcionFisica = dto.DescripcionFisica;
        if (dto.Isbn is not null) libro.Isbn = dto.Isbn;
        if (dto.AnioPublicacion is not null) libro.AnioPublicacion = dto.AnioPublicacion;
        if (dto.Editorial is not null) libro.Editorial = dto.Editorial;

        if (dto.AddAutoresIds is not null)
        {
            var autoresExistentes = await _context.Autores
                .Where(a => dto.AddAutoresIds.Contains(a.AutorId))
                .Select(a => a.AutorId)
                .ToListAsync();

            var autoresNoEncontrados = dto.AddAutoresIds
                .Except(autoresExistentes)
                .ToList();

            if (autoresNoEncontrados.Count != 0)
                throw new NotFoundException("Autores no encontrados", autoresNoEncontrados);

            var autoresYaRelacionados = await _context.AutorLibros
                .Where(al => al.LibroId == id)
                .Select(al => al.AutorId)
                .ToListAsync();

            var nuevasRelaciones = autoresExistentes
                .Except(autoresYaRelacionados)
                .Select(autorId => new AutorLibro
                {
                    AutorId = autorId,
                    LibroId = id
                })
                .ToList();

            if (nuevasRelaciones.Count != 0)
                _context.AutorLibros.AddRange(nuevasRelaciones);
        }

        if (dto.DeleteAutoresIds is not null)
        {
            if (dto.DeleteAutoresIds.Count != 0)
            {
                var autoresRelacionados = await _context.AutorLibros
                    .Where(al => al.LibroId == id)
                    .Select(al => al.AutorId)
                    .ToListAsync();

                var autoresNoRelacionados = dto.DeleteAutoresIds
                    .Except(autoresRelacionados)
                    .ToList();

                if (autoresNoRelacionados.Count != 0)
                    throw new NotFoundException(
                        "Autores no relacionados al libro",
                        autoresNoRelacionados
                    );

                await _context.AutorLibros
                    .Where(al =>
                        al.LibroId == id &&
                        dto.DeleteAutoresIds.Contains(al.AutorId))
                    .ExecuteDeleteAsync();
            }
        }

        // Guardar cambios
        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///     Borrado ligico de un registro (libro) a partir de su ID (SOFT DELETE).
    /// </summary>
    /// <param name="id">
    ///     Valor GUID ID para encontrar el elemento que se quiere borrar.
    /// </param>
    public async Task DeleteLibroAsync(Guid id)
    {
        var _valorBuscado = await _context.Libros.FindAsync(id) 
            ?? throw new NotFoundException($"Libro con id: '{id}' ya fue eliminado.");

        _context.Remove(_valorBuscado);

        await _context.SaveChangesAsync();
    }
}

