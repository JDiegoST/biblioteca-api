using Biblioteca.Domain.Interfaces;
using Biblioteca.Domain.Models.Common;

namespace Biblioteca.Domain.Models;
public class Direccion : BaseModel
{
    public Direccion() { }

    public Guid UsuarioId { get; set; }
    public string Pais { get; set; } = string.Empty;
    public int CodigoPostal { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Colonia { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public short? NumeroInterior { get; set; }
    public int? NumeroExterior { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
