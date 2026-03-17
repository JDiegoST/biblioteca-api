using Biblioteca.Domain.Models.Enums;

namespace Biblioteca.Aplication.DTOs.Ejemplares;
public class CreateNewEjemplarDTO
{
    public Guid LibroId { get; set; }
    public EstadoPrestamo? Estado { get; set; } 
    public string CodigoInventario { get; set; } = string.Empty;
}
