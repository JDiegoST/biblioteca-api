

namespace Biblioteca.Aplication.DTOs.Direcciones;
public class UpdateDomicilioDTO
{
    public string? Pais { get; set; }
    public int? CodigoPostal { get; set; }
    public string? Estado { get; set; } = string.Empty;
    public string? Municipio { get; set; } = string.Empty;
    public string? Colonia { get; set; } = string.Empty;
    public string? Calle { get; set; } = string.Empty;
    public short? NumeroInterior { get; set; }
    public int? NumeroExterior { get; set; }
}
