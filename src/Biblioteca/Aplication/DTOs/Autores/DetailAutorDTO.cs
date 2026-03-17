namespace Biblioteca.Aplication.DTOs.Autores;
public class DetailAutorDTO(string autorName, string apellido)
{

    private readonly string _autorName = autorName;
    private readonly string _apellido = apellido;

    public Guid AutorId { get; set; }
    public string NameComplete { get => $"{_autorName} {_apellido}"; }
    public string Biography { get; set; } = string.Empty;
    public string UrlImg { get; set; } = string.Empty;

}
