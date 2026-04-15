using Microsoft.EntityFrameworkCore;
using Biblioteca.Aplication.DTOs.Autores;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Infrastructure.Context;
using Biblioteca.Domain.Exceptions;
using Biblioteca.Domain.Models;
using Biblioteca.Aplication.DTOs.Libros;
using System.Net.WebSockets;

namespace Biblioteca.Aplication.Services;
public class AutorService(BibliotecaContext _context) : IAutorService
{
   /// <summary>
   ///     Obtiene todos los registros (autores) existentes en la DB.
   ///     Aplica paginacion a la consulta.
   /// </summary>
   /// <param name="pagination">
   ///      Contiene la informacion esencial para aplicar paginacion en la consulta.
   /// </param>
   /// <param name="filters">
   ///      Contiene la informacion esencial de la busqueda concreata en autores.
   /// </param>
   /// <returns>
   ///      Retorna el objeto PagedResult que contiene informacion con la cantidad y los 
   ///      registros encontrados ademas de informacion adicional sobre la paginacion.
   /// </returns>
    public async Task<PagedResult<DetailAutorDTO>> GetAutorsAsync(FilterParams filters, PaginationParams pagination)
    {
        var query = _context.Autores
            .AsNoTracking()
            .OrderBy(a => a.Apellido).AsQueryable();

        if (!String.IsNullOrEmpty(filters.Q))
        {
            query = query.
                    Where(a => EF.Functions.Like(a.Nombre.ToLower() + " " +a.Apellido.ToLower() , $"%{filters.Q.ToLower()}%"));
        }

        var totalItems = await query.CountAsync();

        var autors = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(a => new DetailAutorDTO(a.Nombre, a.Apellido)
            {
                AutorId = a.AutorId,
                Biography = a.Biografia,
                UrlImg = a.UrlImgAutor
            })
            .ToListAsync();

        return new PagedResult<DetailAutorDTO>
        {
            TotalItems = totalItems,
            Items = autors,
            PageSize = pagination.PageSize,
            PageNumber = pagination.PageNumber,
        };
    }

    /// <summary>
    ///     Busca y obtiene un registro (autor) especifico con si ID.
    /// </summary>
    /// <param name="id">
    ///     Valor GUID ID para encontrar el autor buscado.
    /// </param>
    /// <returns>
    ///     Retorna un DTO con la informacion del autor buscado en caso de que exista.
    /// </returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<DetailAutorDTO?> GetAutorByIdAsync(Guid id)
    {
        var autor = await _context.Autores
            .AsNoTracking()
            .Where(a => a.AutorId == id)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException($"El autor id: '{id}' no existe.");

        return new DetailAutorDTO(autor.Nombre, autor.Apellido)
        {
            AutorId = autor.AutorId,
            Biography = autor.Biografia,
            UrlImg = autor.UrlImgAutor 
        };
    }
    
    /// <summary>
    ///     Obtiene todos los libros en lo que un AUTOR participo mediante el ID
    /// </summary>
    /// <param name="autorId">
    ///     Valor GUID ID para buscar el autor especifico.
    /// </param>
    /// <param name="pagination">
    ///     Objeto que contiene todos los datos esenciales para aplicar paginacion a la consulta.
    /// </param>
    /// <returns>
    ///     Retorn el objeto PagedResult el cual ademas de al cantidad y los elmentos encontrados
    ///     , tambien contiene informacion necesaria para la paginacion.
    /// </returns>
    public async Task<PagedResult<SummaryLibroResumenDTO>> GetLibrosOfAutor(Guid autorId, PaginationParams pagination)
    {
        var query = _context.AutorLibros
            .AsNoTracking()
            .Where(au => au.AutorId == autorId);

        var allItems = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(au => new SummaryLibroResumenDTO()
            {
                LibroId = au.LibroId,
                Titulo = au.Libro.Titulo,
            })
            .ToListAsync();

        return new PagedResult<SummaryLibroResumenDTO>()
        {
            TotalItems = allItems,
            Items = items,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

    }

    /// <summary>
    ///     Crea y guarda un nuevo autor en la DB.
    /// </summary>
    /// <param name="autor">
    ///     Contiene toda la informacion necesaria para crear el nuevo registro en la BD.
    /// </param>
    /// <returns>
    ///     Retorna un DTO de respuesta con la informacion del nuevo recurso (autor) creado.
    /// </returns>
    public async Task<DetailAutorDTO> SaveAutorAsync(SaveAutorDTO autor)
    {
        var newAutor = new Autor()
        {
            AutorId = Guid.NewGuid(),
            Nombre = autor.Name,
            Apellido = autor.LastName,
            Biografia = autor.Biography,
            UrlImgAutor = autor.UrlImg
        };

        await _context.AddAsync(newAutor);
        await _context.SaveChangesAsync();

        return new DetailAutorDTO(newAutor.Nombre, newAutor.Apellido)
        {
            AutorId = newAutor.AutorId,
            Biography = newAutor.Biografia,
            UrlImg = newAutor.UrlImgAutor
        };
    }

    /// <summary>
    ///     Actualiza los campos especificos de un registro (autor) especifico.
    /// </summary>
    /// <param name="id">
    ///     Valor GUID ID para encontar el elemento que se quiere modificar.
    /// </param>
    /// <param name="autor">
    ///     DTO que contiene la informacion que se quiere modificar de un registro (autor).
    /// </param>
    public async Task UpdateAutorAsync(Guid id, UpdateAutorDTO autor)
    {
        var specificAutor = await _context.Autores.FindAsync(id)
            ?? throw new NotFoundException($"El autor id: '{id}' no existe.");

        if ( autor.NameAutor != null) specificAutor.Nombre = autor.NameAutor;
        if( autor.LastName != null ) specificAutor.Apellido = autor.LastName;
        if( autor.Bibliografy != null ) specificAutor.Biografia = autor.Bibliografy;
        if( autor.UrlImg != null ) specificAutor.UrlImgAutor = autor.UrlImg;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///     Borrado ligico de un registro (autor) especifico (SOFT DELETE).
    /// </summary>
    /// <param name="id">
    ///     Valor GUID ID para encontrar el registro (autor) especifico.
    /// </param>
    public async Task DeleteAutorAsync(Guid id)
    {
        var _valueFound = await _context.Autores.FindAsync(id)
            ?? throw new NotFoundException($"El autor id: '{id}' ya no existe.");

        _context.Remove(_valueFound);

        await _context.SaveChangesAsync();
    }
}
