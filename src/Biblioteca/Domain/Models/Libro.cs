using Biblioteca.Domain.Models.Common;
using System.Text.Json.Serialization;

namespace Biblioteca.Domain.Models;
public class Libro : BaseModel
{
    public Libro() { }

    public Guid LibroId { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public string RutaImagen { get; set; } = string.Empty;
    public string DescripcionFisica { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;

    public int? AnioPublicacion { get; set; }

    public string Editorial { get; set; } = string.Empty;

    public ICollection<AutorLibro> AutoresLibros { get; set; } = [];
    public ICollection<Ejemplar> Ejemplares { get; set; } = [];

    public override string ToString()
    {
        return $"Recurso: {LibroId}, Titulo: {Titulo}, Editorial: {Editorial}";
    }
}
