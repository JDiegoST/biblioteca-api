
namespace Biblioteca.Aplication.DTOs.Prestamos;
public class CreateNewPrestamoDTO
{
    public Guid EjemplarId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaVencimiento { get; set; }
}
