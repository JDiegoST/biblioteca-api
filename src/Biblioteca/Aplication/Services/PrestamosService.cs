using Microsoft.EntityFrameworkCore;
using Biblioteca.Aplication.DTOs.common;
using Biblioteca.Aplication.DTOs.Prestamos;
using Biblioteca.Aplication.Interfaces;
using Biblioteca.Infrastructure.Context;
using Biblioteca.Domain.Models;
using Biblioteca.Aplication.DTOs.Libros;
using Biblioteca.Aplication.DTOs.Usuarios;
using Biblioteca.Aplication.DTOs.Ejemplares;
using Biblioteca.Domain.Exceptions;
using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Aplication.Services;
public class PrestamosService(BibliotecaContext _context) : IPrestamoService
{
    /// <summary>
    ///     Recopila todos los prestamos existentes en la base de datos.
    ///     Aplica paginacion en la consulta.
    /// </summary>
    /// <param name="pagination">
    ///     Representa DTO necesario para aplicar una correcta paginacion a el resultado de la consulta.
    /// </param>
    /// <return>
    ///     Retorna un objeto PagedResult con la informacion necesario para la paginacion
    ///     ademas de la lista de todos los prestamos disponibles en la base de datos.
    /// </return>
    public async Task<PagedResult<DetailPrestamoDTO>> GetAllLoansAsync(PaginationParams pagination)
    {
        var query = _context.Prestamos
            .AsNoTracking()
            .OrderBy(p => p.UsuarioId);

        var totalItems = await query.CountAsync();

        var prestamos = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(l => new DetailPrestamoDTO()
            {
                PrestamoId = l.PrestamoId,
                Ejemplar = new DetailEjemplarDTO()
                {
                    EjemplarId = l.Ejemplar.EjemplarId,
                    CodigoInventario = l.Ejemplar.CodigoInventario,
                    Estado = l.Ejemplar.Estado,
                    Libro = new SummaryLibroResumenDTO()
                    {
                        LibroId = l.Ejemplar.Libro.LibroId,
                        Titulo = l.Ejemplar.Libro.Titulo
                    },
                },
                Usuario = new SummaryUsuarioDTO()
                {
                    NombreCompleto = l.Usuario.NombreCompleto,
                    UsuarioId = l.Usuario.UsuarioId,
                },
                FechaPrestamo = l.FechaPrestamo,
                FechaVencimiento = l.FechaVencimiento,
                FechaDevolucion = l.FechaDevolucion,
            }).ToListAsync();

        return new PagedResult<DetailPrestamoDTO>()
        {
            TotalItems = totalItems,
            Items = prestamos,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
        };
    }

    /// <summary>
    ///     Busca un elemento en la base de datos a partir de su ID
    /// </summary>
    /// <param name="id">
    ///     Valor GUID del ID para buscar el prestamo especifico.
    /// </param>
    /// <returns>
    ///     Retorna un DetailEjemplarDTO con la informacion en caso que el
    ///     elemento exista en la base de datos a partir del ID.
    /// </returns>
    public async Task<DetailPrestamoDTO?> GetLoanByIdAsync(Guid id)
    {
        var elementFound = await _context.Prestamos.Select(l => new DetailPrestamoDTO()
        {
            PrestamoId = l.PrestamoId,
            Ejemplar = new DetailEjemplarDTO()
            {
                EjemplarId = l.Ejemplar.EjemplarId,
                CodigoInventario = l.Ejemplar.CodigoInventario,
                Estado = l.Ejemplar.Estado,
                Libro = new SummaryLibroResumenDTO()
                {
                    Titulo = l.Ejemplar.Libro.Titulo
                },
            },
            Usuario = new SummaryUsuarioDTO()
            {
                NombreCompleto = l.Usuario.NombreCompleto,
                UsuarioId = l.Usuario.UsuarioId,
            },
            FechaPrestamo = l.FechaPrestamo,
            FechaVencimiento = l.FechaVencimiento,
            FechaDevolucion = l.FechaDevolucion,
        }).FirstOrDefaultAsync(l => l.PrestamoId == id) ?? throw new NotFoundException($"El prestamo con id: '{id}' no existe.");

        return elementFound;
    }

    /// <summary>
    ///     Crea un nuevo prestamo.
    /// </summary>
    /// <param name="prestamo">
    ///     Representa el DTO que contiene los datos necesario para crear un nuevo prestamo.
    /// </param>
    /// <returns>
    ///     Retorna un DTO para notificar al usuaio el recurso que ha creado.
    /// </returns>
    public async Task<DetailPrestamoDTO> CreateNewLoanAsync(CreateNewPrestamoDTO prestamo)
    {
        // Inicio una transaccion para evitar errores en el cambio de estado
        using var transaction = await _context.Database.BeginTransactionAsync();

        // Verifico que el Id proporcionado del ejemplar exita en el sistema.
        var ejemplar = await _context.Ejemplares.FindAsync(prestamo.EjemplarId)
            ?? throw new NotFoundException($"Ejemplar con el ID: '{prestamo.EjemplarId}' No encontrado.");

        // Verifico que el ejemplar a prestar este disponible para el usuario.
        if (ejemplar.Estado == EstadoPrestamo.Indisponible) throw new ConflictException($"Ejemplar '{prestamo.EjemplarId}', no disponible.");

        // Cambio el estado del ejemplar a No disponible para mantener la lgica de prestamo.
        ejemplar.Estado = EstadoPrestamo.Indisponible;

        // Verifico que el ID del usuario exista en el sistema.
        var usuario = await _context.Usuarios.FindAsync(prestamo.UsuarioId)
            ?? throw new NotFoundException($"Usuario con el ID: '{prestamo.UsuarioId}', no existe.");

        // Creo la insercion
        var elementCreated = new Prestamo()
        {
            PrestamoId = Guid.NewGuid(),
            EjemplarId = prestamo.EjemplarId,
            UsuarioId = prestamo.UsuarioId,
            FechaPrestamo = prestamo.FechaPrestamo,
            FechaVencimiento = prestamo.FechaVencimiento
        };

        await _context.AddAsync(elementCreated);

        // Guardo el registro.
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return await _context.Prestamos.Select( l => new DetailPrestamoDTO()
        {
            PrestamoId = l.PrestamoId,
            Ejemplar = new DetailEjemplarDTO()
            {
                EjemplarId = l.Ejemplar.EjemplarId,
                CodigoInventario = l.Ejemplar.CodigoInventario,
                Estado = l.Ejemplar.Estado,
                Libro = new SummaryLibroResumenDTO()
                {
                    Titulo = l.Ejemplar.Libro.Titulo
                },
            },
            Usuario = new SummaryUsuarioDTO()
            {
                NombreCompleto = l.Usuario.NombreCompleto,
                UsuarioId = l.Usuario.UsuarioId,
            },
            FechaPrestamo = l.FechaPrestamo,
            FechaVencimiento = l.FechaVencimiento,
            FechaDevolucion = l.FechaDevolucion,
        }).FirstAsync(l => l.PrestamoId == elementCreated.PrestamoId);
    }

    /// <summary>
    ///     Actualiza los datos epecificos de un registro a partir de su ID
    /// </summary>
    /// <param name="id">
    ///     Representa el valor GUID que funciona como ID del registro especifico
    /// </param>
    /// <param name="prestamo">
    ///     Contiene el DTO con la informacion que se desea cambiar.
    /// </param>
    public async Task UpdateLoanAsync(Guid id, UpdatePrestamoDTO prestamo)
    {
        var elementToUpdate = await _context.Prestamos.FindAsync(id) 
            ?? throw new NotFoundException($"El prestamo con el Id: '{id}' que intenas modificar, no exite.");

        if (prestamo.FechaVencimiento.HasValue) {
            // Verifico que la extencion del prestamo sea dentro del vencimiento del mismo
            if (elementToUpdate.FechaVencimiento <= DateTime.UtcNow)
                throw new BusinesException("No es posible extender el prestamo debido a que ya ha vencido.");

            if (prestamo.FechaVencimiento <= DateTime.UtcNow)
                throw new BusinesException($"Elige una fecha de vencimiento mayor a la fecha actual");

            elementToUpdate.FechaVencimiento = prestamo.FechaVencimiento.Value;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///     Devuelve logicamente un prestamo Activo. Cambia el estado del ejemplar 
    ///     de NO-DISPONIBLE a DISPONIBLE
    /// </summary>
    /// <param name="id">
    ///     Representa el valor GUID para encontrar el prestamo especifico.
    /// </param>
    public async Task GoBackLoanAsync(Guid id)
    {
        var elementToUpdate = await _context.Prestamos.FindAsync(id)
            ?? throw new NotFoundException($"El prestamo con el Id: '{id}' que intenas devolver, no exite.");

        if (elementToUpdate.FechaDevolucion != null) throw new ConflictException($"Este prestamo ya fue devuelto.");

        // Asigno la hora de devolucion del Ejemplar.
        elementToUpdate.FechaDevolucion = DateTime.UtcNow;

        // Actualizo el estado del ejemplar de No disponible a Disponible.
        var ejemplar = await _context.Ejemplares.Where(e => e.EjemplarId == elementToUpdate.EjemplarId).FirstAsync();
        ejemplar.Estado = EstadoPrestamo.Disponible;
    }
}
