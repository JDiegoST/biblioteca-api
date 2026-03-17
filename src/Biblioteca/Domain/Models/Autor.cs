
using Biblioteca.Domain.Models.Common;

namespace Biblioteca.Domain.Models;
public class Autor : BaseModel
{
    public Autor() { }
  
    public Guid AutorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Biografia { get; set; } = string.Empty;
    public string UrlImgAutor {  get; set; } = string.Empty;

    public ICollection<AutorLibro> AutoresLibros { get; set; } = [];
}
