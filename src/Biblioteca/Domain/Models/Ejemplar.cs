using Biblioteca.Domain.Models.Common;
using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Domain.Models;
public class Ejemplar : BaseModel
{
    public Ejemplar() { }

    public Guid EjemplarId { get; set; }
    public Guid LibroId { get; set; }
    public EstadoPrestamo? Estado { get; set; }
    public string CodigoInventario { get; set; } = string.Empty;

    public Libro Libro { get; set; } = null!;

    public ICollection<Prestamo>? Prestamos { get; set; }
}

