using Biblioteca.Domain.Models.Common;
using System.Text.Json.Serialization;

namespace Biblioteca.Domain.Models;
public class Prestamo : BaseModel
{
    public Prestamo() { }
    
    public Guid PrestamoId { get; set; }
    public Guid EjemplarId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    
    public Ejemplar Ejemplar { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
