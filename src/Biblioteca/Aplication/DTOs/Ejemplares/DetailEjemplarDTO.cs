using Biblioteca.Aplication.DTOs.Libros;
using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Aplication.DTOs.Ejemplares;
public class DetailEjemplarDTO
{
    public Guid EjemplarId { get; set; }
    public string CodigoInventario { get; set; } = null!;
    public EstadoPrestamo? Estado { get; set; } 

    public SummaryLibroResumenDTO Libro { get; set; } = null!;

}
