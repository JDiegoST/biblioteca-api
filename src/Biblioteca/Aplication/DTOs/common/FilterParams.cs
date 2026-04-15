namespace Biblioteca.Aplication.DTOs.common
{
    public class FilterParams : Params
    {
        public string Q { get; set; } = string.Empty;

        public Boolean? Disponible { get; set; }
    }
}
