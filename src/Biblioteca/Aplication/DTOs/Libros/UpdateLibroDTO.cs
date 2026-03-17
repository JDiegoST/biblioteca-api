using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Aplication.DTOs.Libros;

public class UpdateLibroDTO
{
    [StringLength(200)]
    public string? Titulo { get; set; }

    [Url]
    public string? RutaImagen { get; set; }

    [StringLength(500)]
    public string? DescripcionFisica { get; set; }

    [StringLength(13, MinimumLength = 10)]
    [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$",
        ErrorMessage = "ISBN no válido")]
    public string? Isbn { get; set; }

    [Range(1400, 2100)]
    public int? AnioPublicacion { get; set; }

    [StringLength(100)]
    public string? Editorial { get; set; }

    public List<Guid>? AddAutoresIds { get; set; }
    public List<Guid>? DeleteAutoresIds { get; set; }
}

