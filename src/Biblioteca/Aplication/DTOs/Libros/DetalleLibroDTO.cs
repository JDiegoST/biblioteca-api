namespace Biblioteca.Aplication.DTOs.Libros;
public class DetalleLibroDTO
{
    public Guid LibroId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string RutaImagen { get; set; } = string.Empty;
    public string DescripcionFisica { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;

    public int? AnioPublicacion { get; set; }

    public string Editorial { get; set; } = string.Empty;
    public List<string> Autores { get; set; } = [];
}

