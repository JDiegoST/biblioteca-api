using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Aplication.DTOs.Libros;
public class CreateNewLibroDTO 
{
    [Required]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;
    [Url]
    public string RutaImagen { get; set; } = string.Empty;
    [Required]
    [StringLength(200)]
    public string DescripcionFisica { get; set; } = string.Empty;
    [Required]
    [StringLength(13, MinimumLength = 10)]
    [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$",
        ErrorMessage = "ISBN no válido")]
    public string Isbn { get; set; } = string.Empty;

    [Range(1, 9999)]
    public int? AnioPublicacion { get; set; }
    [Required]
    [StringLength(200)]
    public string Editorial { get; set; } = string.Empty;

    public List<Guid> Autores { get; set; } = [];
}

