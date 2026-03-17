namespace Biblioteca.Domain.Models;
public class AutorLibro
{
    public Guid AutorId { get; set; }
    public Guid LibroId { get; set; }


    public Autor Autor { get; set; } = null!;
    public Libro Libro { get; set; } = null!;

}

