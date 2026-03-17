using Biblioteca.Aplication.DTOs.Ejemplares;
using Biblioteca.Aplication.DTOs.Usuarios;

namespace Biblioteca.Aplication.DTOs.Prestamos;
public class DetailPrestamoDTO
{
    public Guid PrestamoId { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaDevolucion { get; set; }

    public DetailEjemplarDTO Ejemplar { get; set; } = null!;
    public SummaryUsuarioDTO Usuario { get; set; } = null!;
}
